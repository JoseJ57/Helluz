using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TuProyecto.Controllers
{
    [Authorize]
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
                .OrderBy(m => m.IdMembresia)
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
            ViewData["UnidadTiempoList"] = new SelectList(Enum.GetValues(typeof(UnidadTiempo)));
            return View(new Membresia());
        }


        // POST: Membresias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Membresia membresia)
        {
            if (ModelState.IsValid)
            {
                if (membresia.DiasPorSemana <= 0)
                    ModelState.AddModelError(nameof(membresia.DiasPorSemana), "Debe ser mayor que 0.");

                if (membresia.Duracion <= 0)
                    ModelState.AddModelError(nameof(membresia.Duracion), "Debe ser mayor que 0.");

                if (membresia.EsPromocion)
                {
                    if (!membresia.FechaActivo.HasValue || !membresia.FechaInactivo.HasValue)
                        ModelState.AddModelError("", "Las promociones deben tener fechas de inicio y fin.");
                    else
                        membresia.Estado = _membresiaService.ValidarEstadoPromocion(membresia);
                }
                else
                {
                    membresia.Estado = true;
                    membresia.FechaActivo = null;
                    membresia.FechaInactivo = null;
                }

                if (ModelState.IsValid)
                {
                    _context.Add(membresia);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = membresia.EsPromocion
                        ? "Promoción creada exitosamente."
                        : "Membresía creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["UnidadTiempoList"] = new SelectList(Enum.GetValues(typeof(UnidadTiempo)), membresia.UnidadTiempo);
            return View(membresia);
        }


        // GET: Membresias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var membresia = await _context.Membresias.FindAsync(id);
            if (membresia == null)
                return NotFound();

            ViewData["UnidadTiempoList"] = new SelectList(Enum.GetValues(typeof(UnidadTiempo)), membresia.UnidadTiempo);
            return View(membresia);
        }

        // POST: Membresias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Membresia membresia)
        {
            if (id != membresia.IdMembresia)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (membresia.DiasPorSemana <= 0)
                        ModelState.AddModelError(nameof(membresia.DiasPorSemana), "Debe ser mayor que 0.");
                    if (membresia.Duracion <= 0)
                        ModelState.AddModelError(nameof(membresia.Duracion), "Debe ser mayor que 0.");

                    if (membresia.EsPromocion)
                    {
                        if (!membresia.FechaActivo.HasValue || !membresia.FechaInactivo.HasValue)
                        {
                            ModelState.AddModelError("", "Las promociones deben tener fechas de inicio y fin.");
                            ViewData["UnidadTiempoList"] = new SelectList(Enum.GetValues(typeof(UnidadTiempo)), membresia.UnidadTiempo);
                            return View(membresia);
                        }
                        membresia.Estado = _membresiaService.ValidarEstadoPromocion(membresia);
                    }
                    else
                    {
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
                    if (!_context.Membresias.Any(e => e.IdMembresia == membresia.IdMembresia))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UnidadTiempoList"] = new SelectList(Enum.GetValues(typeof(UnidadTiempo)), membresia.UnidadTiempo);
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