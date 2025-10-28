using System;
using System.Collections.Generic;
using System.Linq;
using Helluz.Contexto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helluz.Models;
using Helluz.Dto;

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
        public async Task<IActionResult> MarcarAsistenciaAlumno(int idAlumno, string token)
        {
            var asistencia = new AsistenciaAlumno
            {
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                Hora = TimeOnly.FromDateTime(DateTime.Now),
                Estado = EstadoAsistencia.presente,
                IdAlumno = idAlumno
            };
            _context.AsistenciaAlumnos.Add(asistencia);
            await _context.SaveChangesAsync();

            return Content("Asistencia registrada correctamente ✅");
        }
    }
}
