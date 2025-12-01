using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [AllowAnonymous]
    public class AsistenciaController : Controller
    {
        private readonly MyContext _context;

        public AsistenciaController(MyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: /Asistencia
        public async Task<IActionResult> Index()
        {
            // Trae asistencias con datos de alumno/instructor
            var asistenciasAlumnos = await _context.AsistenciaAlumnos
                .Include(a => a.Alumno)
                .OrderByDescending(a => a.Fecha)
                .ThenByDescending(a => a.Hora)
                .ToListAsync();

            var asistenciasInstructores = await _context.AsistenciaInstructor
                .Include(i => i.Instructor)
                .OrderByDescending(i => i.Fecha)
                .ThenByDescending(i => i.Hora)
                .ToListAsync();

            ViewBag.AsistenciasAlumnos = asistenciasAlumnos;
            ViewBag.AsistenciasInstructores = asistenciasInstructores;

            return View();
        }

        // GET: /Asistencia/RegistroAsistencia
        public IActionResult RegistroAsistencia()
        {
            return View();
        }

        // POST: /Asistencia/Registrar
        [HttpPost]
        public async Task<IActionResult> Registrar(string carnet)
        {
            if (string.IsNullOrWhiteSpace(carnet))
                return Json(new { mensaje = "Debe ingresar un carnet válido." });

            var fecha = DateOnly.FromDateTime(DateTime.Now);
            var horaActual = TimeOnly.FromDateTime(DateTime.Now);
            var diaSemana = DateTime.Now.DayOfWeek;

            // ❌ No se registran asistencias sábado o domingo
            if (diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday)
            {
                return Json(new { mensaje = "❌ Hoy no hay clases, no se puede registrar asistencia." });
            }

            // ======================= ALUMNO =======================
            var alumno = await _context.Alumnos
                .Include(a => a.Inscripciones)
                    .ThenInclude(i => i.Horario)
                .Include(a => a.Inscripciones)
                    .ThenInclude(i => i.Membresia)
                .FirstOrDefaultAsync(a => a.Carnet == carnet);

            if (alumno != null)
            {
                var inscripcion = alumno.Inscripciones
                    .FirstOrDefault(i => i.FechaInicio <= fecha && i.FechaFin >= fecha);

                if (inscripcion == null)
                    return Json(new { mensaje = $"❌ El alumno {alumno.Nombre} no tiene inscripción activa hoy." });

                var horario = inscripcion.Horario;
                if (horario == null)
                    return Json(new { mensaje = $"❌ El alumno {alumno.Nombre} no tiene un horario asignado." });

                // ---- Validar que tenga membresía asociada ----
                if (inscripcion.Membresia == null)
                {
                    return Json(new { mensaje = $"❌ La inscripción del alumno {alumno.Nombre} no tiene una membresía asociada." });
                }

                // ---- Validar límite semanal ----
                if (inscripcion.ControlDias >= inscripcion.Membresia.DiasPorSemana)
                {
                    return Json(new
                    {
                        mensaje = $"⚠️ Usted solo puede asistir {inscripcion.Membresia.DiasPorSemana} días por semana."
                    });
                }

                // 🆕 ---- Verificar si ya marcó asistencia hoy ----
                bool yaAsistio = await _context.AsistenciaAlumnos
                    .AnyAsync(a => a.IdAlumno == alumno.IdAlumno && a.Fecha == fecha);

                if (yaAsistio)
                {
                    return Json(new { mensaje = "⚠️ Usted ya marcó asistencia hoy." });
                }

                EstadoAsistencia estado;

                // ---- Buscar si existe algún horario activo a esta hora ----
                bool existeHorario = await _context.Horarios
                    .AnyAsync(h => horaActual >= h.HoraInicio && horaActual <= h.HoraFin);

                if (!existeHorario)
                {
                    return Json(new { mensaje = "⚠️ No existe un horario en esta hora. No se registró asistencia." });
                }

                // ---- Determinar estado ----
                if (horaActual <= horario.HoraInicio.AddMinutes(30))
                    estado = EstadoAsistencia.presente;
                else if (horaActual > horario.HoraInicio.AddMinutes(30) && horaActual <= horario.HoraFin)
                    estado = EstadoAsistencia.tarde;
                else if (horaActual > horario.HoraFin)
                    estado = EstadoAsistencia.falta;
                else
                    estado = EstadoAsistencia.otro;

                // ---- Registrar asistencia ----
                var asistenciaAlumno = new AsistenciaAlumno
                {
                    IdAlumno = alumno.IdAlumno,
                    Fecha = fecha,
                    Hora = horaActual,
                    Estado = estado
                };

                _context.AsistenciaAlumnos.Add(asistenciaAlumno);

                // Si fue presente o tarde, sumar +1 al control semanal
                if (estado == EstadoAsistencia.presente || estado == EstadoAsistencia.tarde)
                {
                    inscripcion.ControlDias += 1;
                    _context.Update(inscripcion);
                }

                await _context.SaveChangesAsync();

                return Json(new { mensaje = $"✅ Asistencia registrada para {alumno.Nombre} como {estado}." });
            }

            // ======================= INSTRUCTOR =======================
            var instructor = await _context.Instructors
                .Include(i => i.Horarios)
                .FirstOrDefaultAsync(i => i.Carnet == carnet);

            if (instructor != null)
            {
                // 🆕 ---- Verificar si ya marcó asistencia hoy ----
                bool yaAsistio = await _context.AsistenciaInstructor
                    .AnyAsync(a => a.IdInstructor == instructor.IdInstructor && a.Fecha == fecha);

                if (yaAsistio)
                {
                    return Json(new { mensaje = "⚠️ Usted ya marcó asistencia hoy." });
                }

                // ❌ No se registran asistencias sábado o domingo
                if (diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday)
                {
                    return Json(new { mensaje = "❌ Hoy no hay clases, no se puede registrar asistencia." });
                }

                // Obtener todos los horarios del instructor para hoy
                var horariosDia = instructor.Horarios.Where(h =>
                    (diaSemana == DayOfWeek.Monday && h.Lunes) ||
                    (diaSemana == DayOfWeek.Tuesday && h.Martes) ||
                    (diaSemana == DayOfWeek.Wednesday && h.Miercoles) ||
                    (diaSemana == DayOfWeek.Thursday && h.Jueves) ||
                    (diaSemana == DayOfWeek.Friday && h.Viernes)).ToList();

                if (!horariosDia.Any())
                {
                    return Json(new { mensaje = "⚠️ Hoy no tienes horarios asignados." });
                }

                // Buscar si está dentro de alguno de sus horarios
                var horarioActivo = horariosDia.FirstOrDefault(h => horaActual >= h.HoraInicio && horaActual <= h.HoraFin);

                if (horarioActivo == null)
                {
                    return Json(new { mensaje = "❌ No puedes registrar asistencia fuera de tu horario." });
                }

                EstadoAsistencia estado;

                if (horaActual <= horarioActivo.HoraInicio.AddMinutes(30))
                    estado = EstadoAsistencia.presente;
                else if (horaActual > horarioActivo.HoraInicio.AddMinutes(30) && horaActual <= horarioActivo.HoraFin)
                    estado = EstadoAsistencia.tarde;
                else
                    estado = EstadoAsistencia.falta;

                var asistenciaInstructor = new AsistenciaInstructor
                {
                    IdInstructor = instructor.IdInstructor,
                    Fecha = fecha,
                    Hora = horaActual,
                    Estado = estado
                };

                _context.AsistenciaInstructor.Add(asistenciaInstructor);
                await _context.SaveChangesAsync();

                return Json(new { mensaje = $"✅ Asistencia registrada para {instructor.Nombre} como {estado}." });
            }

            // Si no se encontró ni alumno ni instructor
            return Json(new { mensaje = "❌ No se encontró ningún alumno o instructor con ese carnet." });
        }
    }
}
