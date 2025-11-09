using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Helluz.Controllers
{
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
            var hora = TimeOnly.FromDateTime(DateTime.Now);

            // Buscar alumno
            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Carnet == carnet);
            if (alumno != null)
            {
                var asistenciaAlumno = new AsistenciaAlumno
                {
                    IdAlumno = alumno.IdAlumno,
                    Fecha = fecha,
                    Hora = hora,
                    Estado = EstadoAsistencia.presente
                };
                _context.AsistenciaAlumnos.Add(asistenciaAlumno);
                await _context.SaveChangesAsync();

                return Json(new { mensaje = $"✅ Asistencia registrada correctamente para el alumno: {alumno.Nombre} ({alumno.Carnet})" });
            }

            // Buscar instructor
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Carnet == carnet);
            if (instructor != null)
            {
                var asistenciaInstructor = new AsistenciaInstructor
                {
                    IdInstructor = instructor.IdInstructor,
                    Fecha = fecha,
                    Hora = hora,
                    Estado = EstadoAsistencia.presente
                };
                _context.AsistenciaInstructor.Add(asistenciaInstructor);
                await _context.SaveChangesAsync();

                return Json(new { mensaje = $"✅ Asistencia registrada correctamente para el instructor: {instructor.Nombre} ({instructor.Carnet})" });
            }

            // No encontrado
            return Json(new { mensaje = "❌ No se encontró ningún alumno o instructor con ese carnet." });
        }
    }
}
