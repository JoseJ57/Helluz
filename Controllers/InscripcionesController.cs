using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helluz.Contexto;
using Helluz.Models;

namespace Helluz.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly MyContext _context;

        public InscripcionesController(MyContext context)
        {
            _context = context;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index()
        {
            var myContext = _context.Inscripcions.Include(i => i.Alumno).Include(i => i.Membresia);
            return View(await myContext.ToListAsync());
        }

        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .FirstOrDefaultAsync(m => m.IdInscripcion == id);
            if (inscripcion == null)
            {
                return NotFound();
            }

            return View(inscripcion);
        }

        // GET: Inscripciones/Create
        public IActionResult Create()
        {
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido");

            ViewBag.Membresias = _context.Membresias
                .Select(m => new
                {
                    m.IdMembresia,
                    m.Nombre,
                    m.DiasPorSemana,
                    m.DuracionSemanas
                })
                .ToList();

            return View();
        }



        // POST: Inscripciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdInscripcion,MetodoPago,NroPermisos,Estado,IdAlumno,IdMembresia")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                // Obtener la membresía seleccionada
                var membresia = await _context.Membresias.FindAsync(inscripcion.IdMembresia);
                if (membresia != null)
                {
                    // Fecha de inicio = hoy
                    inscripcion.FechaInicio = DateOnly.FromDateTime(DateTime.Today);

                    // Calcular fecha fin según días por semana y duración en semanas
                    int totalDias = membresia.DiasPorSemana * membresia.DuracionSemanas;
                    DateTime fechaTemp = DateTime.Today;
                    int diasContados = 0;

                    while (diasContados < totalDias)
                    {
                        fechaTemp = fechaTemp.AddDays(1);
                        if (fechaTemp.DayOfWeek != DayOfWeek.Saturday && fechaTemp.DayOfWeek != DayOfWeek.Sunday)
                        {
                            diasContados++;
                        }
                    }

                    // Convertimos a DateOnly
                    inscripcion.FechaFin = DateOnly.FromDateTime(fechaTemp);
                }

                // Guardar en la base
                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si hay error, recargar los selects
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido", inscripcion.IdAlumno);
            ViewBag.Membresias = _context.Membresias
                .Select(m => new
                {
                    m.IdMembresia,
                    m.Nombre,
                    m.Nro_sesiones,
                    m.DiasPorSemana,
                    m.DuracionSemanas
                })
                .ToList();

            return View(inscripcion);
        }


        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripcions.FindAsync(id);
            if (inscripcion == null)
            {
                return NotFound();
            }
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido", inscripcion.IdAlumno);
            ViewData["IdMembresia"] = new SelectList(_context.Membresias, "IdMembresia", "Nombre", inscripcion.IdMembresia);
            return View(inscripcion);
        }

        // POST: Inscripciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInscripcion,FechaInicio,FechaFin,MetodoPago,NroPermisos,Estado,IdAlumno,IdMembresia")] Inscripcion inscripcion)
        {
            if (id != inscripcion.IdInscripcion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscripcionExists(inscripcion.IdInscripcion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "Apellido", inscripcion.IdAlumno);
            ViewData["IdMembresia"] = new SelectList(_context.Membresias, "IdMembresia", "Nombre", inscripcion.IdMembresia);
            return View(inscripcion);
        }

        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .FirstOrDefaultAsync(m => m.IdInscripcion == id);
            if (inscripcion == null)
            {
                return NotFound();
            }

            return View(inscripcion);
        }

        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripcions.FindAsync(id);
            if (inscripcion != null)
            {
                _context.Inscripcions.Remove(inscripcion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripcions.Any(e => e.IdInscripcion == id);
        }
    }
}
