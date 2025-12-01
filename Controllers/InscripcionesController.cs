using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [Authorize(Roles = "administrador")]
    public class InscripcionesController : Controller
    {
        private readonly MyContext _context;

        public InscripcionesController(MyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Actualiza el estado de la inscripción según la fecha actual.
        /// </summary>
        private void ActualizarEstadoInscripcion(Inscripcion inscripcion)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (inscripcion.FechaInicio <= hoy && hoy <= inscripcion.FechaFin)
                inscripcion.Estado = EstadoInscripcion.Activa;
            else
                inscripcion.Estado = EstadoInscripcion.Vencida;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("administrador") && !User.IsInRole("instructor"))
            {
                return Unauthorized();
            }

            var inscripciones = await _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .Include(i => i.Horario)
                .ToListAsync();

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var diaSemana = DateTime.Today.DayOfWeek;
            bool cambios = false;

            foreach (var inscripcion in inscripciones)
            {
                // Actualizar estado de inscripción
                var estadoAnterior = inscripcion.Estado;
                if (inscripcion.FechaInicio <= hoy && hoy <= inscripcion.FechaFin)
                    inscripcion.Estado = EstadoInscripcion.Activa;
                else
                    inscripcion.Estado = EstadoInscripcion.Vencida;

                if (inscripcion.Estado != estadoAnterior)
                {
                    _context.Update(inscripcion);
                    cambios = true;
                }

                // 🔹 Reiniciar ControlDias los lunes si la inscripción está activa
                if (diaSemana == DayOfWeek.Monday && inscripcion.Estado == EstadoInscripcion.Activa && inscripcion.ControlDias != 0)
                {
                    inscripcion.ControlDias = 0;
                    _context.Update(inscripcion);
                    cambios = true;
                }
            }

            if (cambios)
                await _context.SaveChangesAsync();

            return View(inscripciones);
        }

        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var inscripcion = await _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .Include(i => i.Horario)
                .FirstOrDefaultAsync(m => m.IdInscripcion == id);

            if (inscripcion == null) return NotFound();

            return View(inscripcion);
        }

        // GET: Inscripciones/Create
        public IActionResult Create()
        {
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido");
            ViewData["IdHorario"] = new SelectList(
                _context.Horarios
                    .OrderBy(h => h.HoraInicio)
                    .Select(h => new { h.IdHorario, Display = h.HoraInicio + " - " + h.HoraFin }),
                "IdHorario", "Display");

            ViewBag.Membresias = _context.Membresias
                .Select(m => new
                {
                    m.IdMembresia,
                    m.Nombre,
                    m.DiasPorSemana,
                    m.Duracion,
                    m.UnidadTiempo
                })
                .ToList();

            return View();
        }

        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdInscripcion,FechaInicio,FechaFin,MetodoPago,NroPermisos,Estado,IdAlumno,IdMembresia,IdHorario")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                // Inicializar ControlDias en 0 al crear la inscripción
                inscripcion.ControlDias = 0;

                // Actualiza el estado según fechas
                ActualizarEstadoInscripcion(inscripcion);

                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido", inscripcion.IdAlumno);
            ViewData["IdHorario"] = new SelectList(
                _context.Horarios.Select(h => new { h.IdHorario, Display = h.HoraInicio + " - " + h.HoraFin }),
                "IdHorario", "Display",
                inscripcion.IdHorario);
            ViewBag.Membresias = _context.Membresias.Select(m => new { m.IdMembresia, m.Nombre }).ToList();

            return View(inscripcion);
        }

        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var inscripcion = await _context.Inscripcions.FindAsync(id);
            if (inscripcion == null) return NotFound();

            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido", inscripcion.IdAlumno);
            ViewData["IdHorario"] = new SelectList(
                _context.Horarios.Select(h => new { h.IdHorario, Display = h.HoraInicio + " - " + h.HoraFin }),
                "IdHorario", "Display",
                inscripcion.IdHorario);
            ViewBag.Membresias = _context.Membresias
                .Select(m => new { m.IdMembresia, m.Nombre, m.DiasPorSemana, m.Duracion })
                .ToList();

            return View(inscripcion);
        }

        // POST: Inscripciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInscripcion,FechaInicio,FechaFin,MetodoPago,NroPermisos,Estado,IdAlumno,IdMembresia,IdHorario")] Inscripcion inscripcion)
        {
            if (id != inscripcion.IdInscripcion) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    ActualizarEstadoInscripcion(inscripcion);
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscripcionExists(inscripcion.IdInscripcion))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido", inscripcion.IdAlumno);
            ViewData["IdHorario"] = new SelectList(
                _context.Horarios.Select(h => new { h.IdHorario, Display = h.HoraInicio + " - " + h.HoraFin }),
                "IdHorario", "Display",
                inscripcion.IdHorario);
            ViewBag.Membresias = _context.Membresias.Select(m => new { m.IdMembresia, m.Nombre }).ToList();

            return View(inscripcion);
        }

        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var inscripcion = await _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .Include(i => i.Horario)
                .FirstOrDefaultAsync(m => m.IdInscripcion == id);

            if (inscripcion == null) return NotFound();

            return View(inscripcion);
        }

        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripcions.FindAsync(id);
            if (inscripcion != null)
                _context.Inscripcions.Remove(inscripcion);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripcions.Any(e => e.IdInscripcion == id);
        }
    }
}
