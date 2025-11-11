using Helluz.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helluz.Models;

namespace Helluz.Controllers
{
    public class HorariosController : Controller
    {
        private readonly MyContext _context;

        public HorariosController(MyContext context)
        {
            _context = context;
        }

        // Método auxiliar para cargar las listas de select
        private void CargarViewBags(Horario horario = null)
        {
            var disciplinas = _context.Disciplinas
                                      .Where(d => d.Estado)
                                      .Select(d => new { d.IdDisciplina, d.NombreDisciplina })
                                      .ToList();

            ViewBag.DisciplinasLunes = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina", horario?.IdDisciplinaLunes);
            ViewBag.DisciplinasMartes = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina", horario?.IdDisciplinaMartes);
            ViewBag.DisciplinasMiercoles = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina", horario?.IdDisciplinaMiercoles);
            ViewBag.DisciplinasJueves = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina", horario?.IdDisciplinaJueves);
            ViewBag.DisciplinasViernes = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina", horario?.IdDisciplinaViernes);

            var instructores = _context.Instructors
                                       .Where(i => i.Estado)
                                       .Select(i => new { i.IdInstructor, NombreCompleto = i.Apellido + ", " + i.Nombre })
                                       .ToList();

            ViewBag.Instructores = new SelectList(instructores, "IdInstructor", "NombreCompleto", horario?.IdInstructor);
        }

        public class HorarioViewModel
        {
            public int IdHorario { get; set; }
            public required string Hora { get; set; } // HoraInicio - HoraFin
            public required string DisciplinaLunes { get; set; }
            public required string DisciplinaMartes { get; set; }
            public required string DisciplinaMiercoles { get; set; }
            public required string DisciplinaJueves { get; set; }
            public required string DisciplinaViernes { get; set; }
            public required string Instructor { get; set; }
        }

        // GET: Horarios
        public async Task<IActionResult> Index()
        {
            // Cargar los horarios con las relaciones necesarias
            var horarios = await _context.Horarios
                                         .Include(h => h.Instructor)
                                         .Include(h => h.DisciplinaLunes)
                                         .Include(h => h.DisciplinaMartes)
                                         .Include(h => h.DisciplinaMiercoles)
                                         .Include(h => h.DisciplinaJueves)
                                         .Include(h => h.DisciplinaViernes)
                                         .OrderBy(h => h.HoraInicio) // <-- ordenar por HoraInicio
                                         .ToListAsync();

            // Mapear a HorarioViewModel
            var model = horarios.Select(h => new HorarioViewModel
            {
                IdHorario = h.IdHorario,
                Hora = h.HoraInicio.ToString("HH:mm") + " - " + h.HoraFin.ToString("HH:mm"),
                DisciplinaLunes = h.DisciplinaLunes?.NombreDisciplina ?? "",
                DisciplinaMartes = h.DisciplinaMartes?.NombreDisciplina ?? "",
                DisciplinaMiercoles = h.DisciplinaMiercoles?.NombreDisciplina ?? "",
                DisciplinaJueves = h.DisciplinaJueves?.NombreDisciplina ?? "",
                DisciplinaViernes = h.DisciplinaViernes?.NombreDisciplina ?? "",
                Instructor = h.Instructor != null ? $"{h.Instructor.Apellido}, {h.Instructor.Nombre}" : ""
            }).ToList();


            return View(model);
        }

        // GET: Horarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Traer el horario con instructor y disciplinas de cada día
            var horario = await _context.Horarios
                .Include(h => h.Instructor)
                .Include(h => h.DisciplinaLunes)
                .Include(h => h.DisciplinaMartes)
                .Include(h => h.DisciplinaMiercoles)
                .Include(h => h.DisciplinaJueves)
                .Include(h => h.DisciplinaViernes)
                .FirstOrDefaultAsync(h => h.IdHorario == id);

            if (horario == null)
                return NotFound();

            // Crear ViewModel para la vista Details
            var vm = new HorarioViewModel
            {
                IdHorario = horario.IdHorario,
                Hora = $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFin:hh\\:mm}",
                DisciplinaLunes = horario.DisciplinaLunes?.NombreDisciplina ?? "No asignado",
                DisciplinaMartes = horario.DisciplinaMartes?.NombreDisciplina ?? "No asignado",
                DisciplinaMiercoles = horario.DisciplinaMiercoles?.NombreDisciplina ?? "No asignado",
                DisciplinaJueves = horario.DisciplinaJueves?.NombreDisciplina ?? "No asignado",
                DisciplinaViernes = horario.DisciplinaViernes?.NombreDisciplina ?? "No asignado",
                Instructor = horario.Instructor != null ? $"{horario.Instructor.Apellido}, {horario.Instructor.Nombre}" : "No asignado"
            };

            return View(vm);
        }

        // GET: Horarios/Create
        public IActionResult Create()
        {
            // Traer disciplinas activas
            var disciplinas = _context.Disciplinas
                                      .Where(d => d.Estado)
                                      .Select(d => new { d.IdDisciplina, d.NombreDisciplina })
                                      .ToList();

            ViewBag.Disciplinas = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina");

            // Traer instructores activos
            var instructores = _context.Instructors
                                       .Where(i => i.Estado)
                                       .Select(i => new { i.IdInstructor, NombreCompleto = i.Apellido + ", " + i.Nombre })
                                       .ToList();
            ViewBag.Instructores = new SelectList(instructores, "IdInstructor", "NombreCompleto");

            return View();
        }


        // Create: POST: Horarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHorario,HoraInicio,HoraFin,Lunes,Martes,Miercoles,Jueves,Viernes,IdDisciplinaLunes,IdDisciplinaMartes,IdDisciplinaMiercoles,IdDisciplinaJueves,IdDisciplinaViernes,IdInstructor")] Horario horario)
        {
            if (horario.HoraInicio >= horario.HoraFin)
            {
                ModelState.AddModelError("HoraFin", "La hora de fin debe ser mayor que la hora de inicio.");
            }

            // 🔹 Validar solapamiento con otros horarios (GLOBAL)
            var existeSolapamiento = await _context.Horarios
                .AnyAsync(h =>
                    (
                        (horario.HoraInicio < h.HoraFin) &&  // comienza antes de que termine el otro
                        (horario.HoraFin > h.HoraInicio)     // termina después de que empieza el otro
                    )
                );

            if (existeSolapamiento)
            {
                ModelState.AddModelError("HoraInicio", "Ya existe un horario que se solapa con el intervalo especificado.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(horario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Volver a cargar los selects si hay error
            var disciplinas = _context.Disciplinas
                .Where(d => d.Estado)
                .Select(d => new { d.IdDisciplina, d.NombreDisciplina })
                .ToList();

            ViewBag.Disciplinas = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina");

            var instructores = _context.Instructors
                .Where(i => i.Estado)
                .Select(i => new { i.IdInstructor, NombreCompleto = i.Apellido + ", " + i.Nombre })
                .ToList();

            ViewBag.Instructores = new SelectList(instructores, "IdInstructor", "NombreCompleto", horario.IdInstructor);

            return View(horario);
        }

        // GET: Horarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _context.Horarios.FindAsync(id);
            if (horario == null)
            {
                return NotFound();
            }

            // Marcar qué días están activos según si hay disciplina asignada
            horario.Lunes = horario.IdDisciplinaLunes.HasValue;
            horario.Martes = horario.IdDisciplinaMartes.HasValue;
            horario.Miercoles = horario.IdDisciplinaMiercoles.HasValue;
            horario.Jueves = horario.IdDisciplinaJueves.HasValue;
            horario.Viernes = horario.IdDisciplinaViernes.HasValue;

            // Cargar listas de select para la vista
            CargarViewBags(horario);

            return View(horario);
        }


        // POST: Horarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHorario,HoraInicio,HoraFin,Lunes,IdDisciplinaLunes,Martes,IdDisciplinaMartes,Miercoles,IdDisciplinaMiercoles,Jueves,IdDisciplinaJueves,Viernes,IdDisciplinaViernes,IdInstructor")] Horario horario)
        {
            if (id != horario.IdHorario)
            {
                return NotFound();
            }

            // 🔸 Validar que la hora de inicio sea menor que la de fin
            if (horario.HoraInicio >= horario.HoraFin)
            {
                ModelState.AddModelError("HoraFin", "La hora de fin debe ser mayor que la hora de inicio.");
            }

            // 🔸 Validar solapamiento con otros horarios (GLOBAL, ignorando el actual)
            var existeSolapamiento = await _context.Horarios
                .AnyAsync(h =>
                    h.IdHorario != horario.IdHorario && // ignorar el horario que estamos editando
                    (
                        (horario.HoraInicio < h.HoraFin) &&  // empieza antes de que termine el otro
                        (horario.HoraFin > h.HoraInicio)     // termina después de que empieza el otro
                    )
                );

            if (existeSolapamiento)
            {
                ModelState.AddModelError("HoraInicio", "Ya existe un horario que se solapa con el intervalo especificado.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(horario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HorarioExists(horario.IdHorario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // 🔸 Si hay errores, recargamos los select lists
            var disciplinas = _context.Disciplinas
                .Where(d => d.Estado)
                .Select(d => new { d.IdDisciplina, d.NombreDisciplina })
                .ToList();

            ViewBag.Disciplinas = new SelectList(disciplinas, "IdDisciplina", "NombreDisciplina");

            var instructores = _context.Instructors
                .Where(i => i.Estado)
                .Select(i => new { i.IdInstructor, NombreCompleto = i.Apellido + ", " + i.Nombre })
                .ToList();

            ViewBag.Instructores = new SelectList(instructores, "IdInstructor", "NombreCompleto", horario.IdInstructor);

            return View(horario);
        }

        // GET: Horarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Traer el horario con instructor y disciplinas de cada día
            var horario = await _context.Horarios
                .Include(h => h.Instructor)
                .Include(h => h.DisciplinaLunes)
                .Include(h => h.DisciplinaMartes)
                .Include(h => h.DisciplinaMiercoles)
                .Include(h => h.DisciplinaJueves)
                .Include(h => h.DisciplinaViernes)
                .FirstOrDefaultAsync(h => h.IdHorario == id);

            if (horario == null)
                return NotFound();

            // Crear ViewModel para la vista Delete
            var vm = new HorarioViewModel
            {
                IdHorario = horario.IdHorario,
                Hora = $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFin:hh\\:mm}",
                DisciplinaLunes = horario.DisciplinaLunes?.NombreDisciplina ?? "No asignado",
                DisciplinaMartes = horario.DisciplinaMartes?.NombreDisciplina ?? "No asignado",
                DisciplinaMiercoles = horario.DisciplinaMiercoles?.NombreDisciplina ?? "No asignado",
                DisciplinaJueves = horario.DisciplinaJueves?.NombreDisciplina ?? "No asignado",
                DisciplinaViernes = horario.DisciplinaViernes?.NombreDisciplina ?? "No asignado",
                Instructor = horario.Instructor != null ? $"{horario.Instructor.Apellido}, {horario.Instructor.Nombre}" : "No asignado"
            };

            return View(vm);
        }

        // POST: Horarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);
            if (horario != null)
            {
                _context.Horarios.Remove(horario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HorarioExists(int id)
        {
            return _context.Horarios.Any(e => e.IdHorario == id);
        }
    }
}
