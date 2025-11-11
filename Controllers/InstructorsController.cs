using Helluz.Contexto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [Authorize]
    public class InstructorsController : Controller
    {
        private readonly MyContext _context;

        public InstructorsController(MyContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index()
        {
            var instructors = _context.Instructors.Include(i => i.Usuario);
            return View(await instructors.ToListAsync());
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(i => i.IdInstructor == id);

            if (instructor == null) return NotFound();

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreUsuario");
            return View();
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdInstructor,Nombre,Apellido,Carnet,FechaNacimiento,Correo,Celular,Estado,UsuarioId")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                // ---- Validar duplicado de carnet en Instructores ----
                bool carnetEnInstructores = await _context.Instructors.AnyAsync(i => i.Carnet == instructor.Carnet);

                // ---- Validar duplicado de carnet en Alumnos ----
                bool carnetEnAlumnos = await _context.Alumnos.AnyAsync(a => a.Carnet == instructor.Carnet);

                if (carnetEnInstructores || carnetEnAlumnos)
                {
                    ModelState.AddModelError("Carnet", $"❌ El carnet {instructor.Carnet} ya está registrado en otro instructor o alumno.");
                    var listaUsuarios = await _context.Usuarios.ToListAsync();
                    ViewData["UsuarioId"] = new SelectList(listaUsuarios, "IdUsuario", "NombreUsuario", instructor.UsuarioId);
                    return View(instructor);
                }

                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var usuariosLista = await _context.Usuarios.ToListAsync();
            ViewData["UsuarioId"] = new SelectList(usuariosLista, "IdUsuario", "NombreUsuario", instructor.UsuarioId);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            var usuarios = await _context.Usuarios.ToListAsync();

            // Creamos la lista con una opción vacía
            ViewData["UsuarioId"] = new SelectList(
                usuarios,
                "IdUsuario",
                "NombreUsuario",
                instructor.UsuarioId
            );

            return View(instructor);
        }


        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInstructor,Nombre,Apellido,Carnet,FechaNacimiento,Correo,Celular,Estado,UsuarioId")] Instructor instructor)
        {
            if (id != instructor.IdInstructor) return NotFound();

            if (!ModelState.IsValid)
            {
                var usuariosLista = await _context.Usuarios.ToListAsync();
                ViewData["UsuarioId"] = new SelectList(usuariosLista, "IdUsuario", "NombreUsuario", instructor.UsuarioId);
                return View(instructor);
            }

            try
            {
                var instructorDb = await _context.Instructors.FindAsync(id);
                if (instructorDb == null) return NotFound();

                // ---- Validar duplicado de carnet en Instructores (excepto este) ----
                bool carnetEnInstructores = await _context.Instructors
                    .AnyAsync(i => i.Carnet == instructor.Carnet && i.IdInstructor != id);

                // ---- Validar duplicado de carnet en Alumnos ----
                bool carnetEnAlumnos = await _context.Alumnos
                    .AnyAsync(a => a.Carnet == instructor.Carnet);

                if (carnetEnInstructores || carnetEnAlumnos)
                {
                    ModelState.AddModelError("Carnet", $"❌ El carnet {instructor.Carnet} ya está registrado en otro instructor o alumno.");
                    var usuariosLista = await _context.Usuarios.ToListAsync();
                    ViewData["UsuarioId"] = new SelectList(usuariosLista, "IdUsuario", "NombreUsuario", instructor.UsuarioId);
                    return View(instructor);
                }

                // Actualizar propiedades
                instructorDb.Nombre = instructor.Nombre;
                instructorDb.Apellido = instructor.Apellido;
                instructorDb.Carnet = instructor.Carnet;
                instructorDb.FechaNacimiento = instructor.FechaNacimiento;
                instructorDb.Correo = instructor.Correo;
                instructorDb.Celular = instructor.Celular;
                instructorDb.Estado = instructor.Estado;
                instructorDb.UsuarioId = instructor.UsuarioId;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Instructors.Any(e => e.IdInstructor == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(i => i.IdInstructor == id);

            if (instructor == null) return NotFound();

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.IdInstructor == id);
        }
    }
}
