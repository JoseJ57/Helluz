using Helluz.Contexto;
using Helluz.Dto;
using Microsoft.EntityFrameworkCore;

public class ReinicioControlDiasService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReinicioControlDiasService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var hoy = DateTime.Today;

            // Ejecutar solo los lunes a las 00:00
            if (hoy.DayOfWeek == DayOfWeek.Monday)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MyContext>();

                    var inscripciones = await context.Inscripcions
                        .Where(i => i.Estado == EstadoInscripcion.Activa)
                        .ToListAsync();

                    foreach (var ins in inscripciones)
                        ins.ControlDias = 0;

                    await context.SaveChangesAsync();
                }

                // Esperar hasta el próximo lunes
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
            }
            else
            {
                // Revisar de nuevo en 24 horas
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
