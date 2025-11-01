using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helluz.Contexto;
using Helluz.Models;


namespace TuProyecto.Controllers
{
    public class MembresiasController : Controller
    {
        private readonly MyContext _context;
        private readonly MembresiaService _membresiaService;

        public MembresiasController(
            MyContext context,
            MembresiaService membresiaService)
        {
            _context = context;
            _membresiaService = membresiaService;
        }

        // GET: Membresias
        public async Task<IActionResult> Index()
        {
            var membresias = await _context.Membresias
                .OrderByDescending(m => m.IdMembresia)
                .ToListAsync();

            // Actualizar estados en tiempo real
            foreach (var m in membresias)
            {
                if (m.EsPromocion)
                {
                    m.Estado = _membresiaService.ValidarEstadoPromocion(m);
                }
            }

            return View(membresias);
        }

        // GET: Membresias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membresia = await _context.Membresias
                .Include(m => m.Inscripcion)
                .FirstOrDefaultAsync(m => m.IdMembresia == id);

            if (membresia == null)
            {
                return NotFound();
            }

            // Actualizar estado si es promoción
            if (membresia.EsPromocion)
            {
                membresia.Estado = _membresiaService.ValidarEstadoPromocion(membresia);
            }

            return View(membresia);
        }

        // GET: Membresias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Membresias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Membresia membresia)
        {
            if (ModelState.IsValid)
            {
                // Validación para promociones
                if (membresia.EsPromocion)
                {
                    if (!membresia.FechaActivo.HasValue || !membresia.FechaInactivo.HasValue)
                    {
                        ModelState.AddModelError("", "Las promociones deben tener fechas de inicio y fin.");
                        return View(membresia);
                    }

                    // Validar que la fecha de inicio sea antes que la de fin (en el mismo año)
                    var inicioMesDia = new DateTime(2000, membresia.FechaActivo.Value.Month, membresia.FechaActivo.Value.Day);
                    var finMesDia = new DateTime(2000, membresia.FechaInactivo.Value.Month, membresia.FechaInactivo.Value.Day);

                    // Calcular el estado inicial basado en las fechas
                    membresia.Estado = _membresiaService.ValidarEstadoPromocion(membresia);
                }
                else
                {
                    // Membresías normales: siempre activas, sin fechas
                    membresia.Estado = true;
                    membresia.FechaActivo = null;
                    membresia.FechaInactivo = null;
                }

                _context.Add(membresia);
                await _context.SaveChangesAsync();

                TempData["Success"] = membresia.EsPromocion
                    ? "Promoción creada exitosamente."
                    : "Membresía creada exitosamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(membresia);
        }

        // GET: Membresias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membresia = await _context.Membresias.FindAsync(id);

            if (membresia == null)
            {
                return NotFound();
            }

            return View(membresia);
        }

        // POST: Membresias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Membresia membresia)
        {
            if (id != membresia.IdMembresia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validación para promociones
                    if (membresia.EsPromocion)
                    {
                        if (!membresia.FechaActivo.HasValue || !membresia.FechaInactivo.HasValue)
                        {
                            ModelState.AddModelError("", "Las promociones deben tener fechas de inicio y fin.");
                            return View(membresia);
                        }

                        // Recalcular el estado basado en las fechas
                        membresia.Estado = _membresiaService.ValidarEstadoPromocion(membresia);
                    }
                    else
                    {
                        // Membresías normales
                        membresia.Estado = true;
                        membresia.FechaActivo = null;
                        membresia.FechaInactivo = null;
                    }

                    _context.Update(membresia);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Registro actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembresiaExists(membresia.IdMembresia))
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

            return View(membresia);
        }

        // GET: Membresias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membresia = await _context.Membresias
                .Include(m => m.Inscripcion)
                .FirstOrDefaultAsync(m => m.IdMembresia == id);

            if (membresia == null)
            {
                return NotFound();
            }

            return View(membresia);
        }

        // POST: Membresias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membresia = await _context.Membresias
                .Include(m => m.Inscripcion)
                .FirstOrDefaultAsync(m => m.IdMembresia == id);

            if (membresia == null)
            {
                return NotFound();
            }

            // Validar si tiene inscripciones
            if (membresia.Inscripcion.Any())
            {
                TempData["Error"] = "No se puede eliminar esta membresía porque tiene inscripciones asociadas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Membresias.Remove(membresia);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registro eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool MembresiaExists(int id)
        {
            return _context.Membresias.Any(e => e.IdMembresia == id);
        }
    }
}