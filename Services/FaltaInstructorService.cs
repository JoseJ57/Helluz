using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public class FaltaInstructorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public FaltaInstructorService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyContext>();

                var fechaHoy = DateOnly.FromDateTime(DateTime.Now);
                var horaAhora = TimeOnly.FromDateTime(DateTime.Now);
                var diaSemana = DateTime.Now.DayOfWeek;

                // Traer todos los instructores con sus horarios del día
                var instructores = await context.Instructors
                    .Include(i => i.Horarios)
                    .ToListAsync();

                foreach (var instructor in instructores)
                {
                    // Obtener los horarios de hoy
                    var horariosDia = instructor.Horarios.Where(h =>
                        (diaSemana == DayOfWeek.Monday && h.Lunes) ||
                        (diaSemana == DayOfWeek.Tuesday && h.Martes) ||
                        (diaSemana == DayOfWeek.Wednesday && h.Miercoles) ||
                        (diaSemana == DayOfWeek.Thursday && h.Jueves) ||
                        (diaSemana == DayOfWeek.Friday && h.Viernes)).ToList();

                    foreach (var horario in horariosDia)
                    {
                        // Hora para marcar falta = 1 minuto después de HoraFin
                        var horaFalta = horario.HoraFin.AddMinutes(1);

                        if (horaAhora >= horaFalta)
                        {
                            // Revisar si ya registró asistencia en ese horario
                            bool yaAsistio = await context.AsistenciaInstructor
                                .AnyAsync(a =>
                                    a.IdInstructor == instructor.IdInstructor &&
                                    a.Fecha == fechaHoy &&
                                    a.Hora >= horario.HoraInicio &&
                                    a.Hora <= horario.HoraFin &&
                                    (a.Estado == EstadoAsistencia.presente ||
                                     a.Estado == EstadoAsistencia.tarde ||
                                     a.Estado == EstadoAsistencia.permiso));

                            if (!yaAsistio)
                            {
                                // Registrar falta
                                var asistencia = new AsistenciaInstructor
                                {
                                    IdInstructor = instructor.IdInstructor,
                                    Fecha = fechaHoy,
                                    Hora = horaFalta,
                                    Estado = EstadoAsistencia.falta
                                };

                                context.AsistenciaInstructor.Add(asistencia);
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();
            }

            // Revisar cada minuto
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
