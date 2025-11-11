using System;
using System.Collections.Generic;
using System.Linq;
using Helluz.Contexto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;

namespace Helluz.Controllers
{
    [Authorize(Roles = "administrador")]
    public class AlumnoesController : Controller
    {
        private readonly MyContext _context;

        public AlumnoesController(MyContext context)
        {
            _context = context;
        }

        // GET: Alumnoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Alumnos.ToListAsync());
        }

        // GET: Alumnoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .FirstOrDefaultAsync(m => m.IdAlumno == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumnoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Alumnoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAlumno,Nombre,Apellido,Carnet,FechaNacimiento,Celular,NroEmergencia,Correo,Estado")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                // ---- Validar duplicado de carnet en Alumnos ----
                bool carnetEnAlumnos = await _context.Alumnos.AnyAsync(a => a.Carnet == alumno.Carnet);

                // ---- Validar duplicado de carnet en Instructores ----
                bool carnetEnInstructores = await _context.Instructors.AnyAsync(i => i.Carnet == alumno.Carnet);

                if (carnetEnAlumnos || carnetEnInstructores)
                {
                    ModelState.AddModelError("Carnet", $"❌ El carnet {alumno.Carnet} ya está registrado en otro alumno o instructor.");
                    return View(alumno);
                }

                _context.Add(alumno);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Alumno registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Ocurrió un error al registrar el alumno. Verifique los datos.";
            return View(alumno);
        }

        // GET: Alumnoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }
            return View(alumno);
        }

        // POST: Alumnoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAlumno,Nombre,Apellido,Carnet,FechaNacimiento,Celular,NroEmergencia,Correo,Estado")] Alumno alumno)
        {
            if (id != alumno.IdAlumno)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // ---- Validar duplicado de carnet en Alumnos (excepto el actual) ----
                bool carnetEnAlumnos = await _context.Alumnos
                    .AnyAsync(a => a.Carnet == alumno.Carnet && a.IdAlumno != alumno.IdAlumno);

                // ---- Validar duplicado de carnet en Instructores ----
                bool carnetEnInstructores = await _context.Instructors
                    .AnyAsync(i => i.Carnet == alumno.Carnet);

                if (carnetEnAlumnos || carnetEnInstructores)
                {
                    ModelState.AddModelError("Carnet", $"❌ El carnet {alumno.Carnet} ya está registrado en otro alumno o instructor.");
                    return View(alumno);
                }

                try
                {
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumnoExists(alumno.IdAlumno))
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
            return View(alumno);
        }

        // GET: Alumnoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .FirstOrDefaultAsync(m => m.IdAlumno == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumnoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno != null)
            {
                _context.Alumnos.Remove(alumno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.IdAlumno == id);
        }
    }
}
