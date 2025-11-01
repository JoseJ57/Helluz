using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly MyContext _context;
        public IActionResult Index()
        {
            return View();
        }
        // Vista a la que el QR dirige
        public async Task<IActionResult> RegistrarAsistencia(string token)
        {
            if (token == null)
                return NotFound();

            var tokenQr = await _context.TokenQrs.FirstOrDefaultAsync(t => t.Token == token && t.Estado == true);
            if (tokenQr == null)
                return Content("El QR no es válido o ya expiró.");

            // Aquí puedes mostrar una vista con un botón “Marcar asistencia”
            return View(tokenQr);
        }

        // POST que guarda la asistencia (puede diferenciar entre alumno o instructor)
        [HttpPost]
        public async Task<IActionResult> MarcarAsistencia(string carnet)
        {

            if (string.IsNullOrWhiteSpace(carnet))
                return BadRequest("Debe ingresar un carnet válido.");

            var fecha = DateOnly.FromDateTime(DateTime.Now);
            var hora = TimeOnly.FromDateTime(DateTime.Now);

            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Carnet == carnet);
            if (alumno != null)
            {
                var asistencia = new AsistenciaAlumno
                {
                    Fecha = fecha,
                    Hora = hora,
                    Estado = EstadoAsistencia.presente,
                    IdAlumno=alumno.IdAlumno
                };

                _context.AsistenciaAlumnos.Add(asistencia);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = $"Asistencia registrada correctamente ✅ {alumno.Nombre} ({alumno.Carnet})" });
            }

            // Si no es alumno, buscar instructor
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Carnet == carnet);
            if (instructor != null)
            {
                var asistencia = new AsistenciaInstructor
                {
                    Fecha = fecha,
                    Hora = hora,
                    Estado = EstadoAsistencia.presente,
                    IdInstructor = instructor.IdInstructor
                };

                _context.AsistenciaInstructor.Add(asistencia);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = $"Asistencia registrada correctamente ✅{instructor.Nombre} ({instructor.Carnet})" });
            }

            // Si no se encuentra
            return NotFound("No se encontró a nadie con este carnet.");
        }
    }
}
