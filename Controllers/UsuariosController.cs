using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Helluz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [Authorize(Roles = "administrador")]
    public class UsuariosController : Controller
    {
        private readonly MyContext _context;
        private readonly PasswordService _passwordService;
        public UsuariosController(MyContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(
               Enum.GetValues(typeof(Roles))
                   .Cast<Roles>()
                   .Select(r => new { Id = (int)r, Nombre = r.ToString() }),
               "Id", "Nombre");

            return View();

        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,NombreUsuario,Password,Estado,Rol")] Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                // ✅ Asignar estado automáticamente en el controlador
                usuario.Password = _passwordService.HashPassword(usuario.Password!);

                // Asignar estado automáticamente
                usuario.Estado = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // Recargar roles si hay error
            ViewBag.Roles = new SelectList(
                Enum.GetValues(typeof(Roles))
                    .Cast<Roles>()
                    .Select(r => new { Id = (int)r, Nombre = r.ToString() }),
                "Id", "Nombre");

            return View(usuario);
        
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new SelectList(
                Enum.GetValues(typeof(Roles))
                    .Cast<Roles>()
                    .Select(r => new { Id = (int)r, Nombre = r.ToString() }),
                "Id", "Nombre", usuario.Rol);

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,NombreUsuario,Password,Estado,Rol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener usuario original de la BD
                    var usuarioOriginal = await _context.Usuarios.FindAsync(id);

                    if (usuarioOriginal == null)
                    {
                        return NotFound();
                    }

                    // Actualizar campos
                    usuarioOriginal.NombreUsuario = usuario.NombreUsuario;
                    usuarioOriginal.Estado = usuario.Estado;
                    usuarioOriginal.Rol = usuario.Rol;

                    // ⭐ SOLO actualizar contraseña si se ingresó una nueva
                    if (!string.IsNullOrEmpty(usuario.Password))
                    {
                        // Verificar si la contraseña ya está hasheada (evitar doble hash)
                        if (!usuario.Password.Contains('.'))
                        {
                            usuarioOriginal.Password = _passwordService.HashPassword(usuario.Password);
                        }
                        else
                        {
                            usuarioOriginal.Password = usuario.Password;
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Usuario actualizado exitosamente";
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Error al actualizar el usuario");
                    return View(usuario);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(
                Enum.GetValues(typeof(Roles))
                    .Cast<Roles>()
                    .Select(r => new { Id = (int)r, Nombre = r.ToString() }),
                "Id", "Nombre", usuario.Rol);

            return View(usuario);
        }
        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
