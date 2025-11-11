using Helluz.Contexto;
using Helluz.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Laboratorios.Controllers
{
   
    public class LoginController : Controller
    {
        private readonly MyContext _context;
private readonly IMemoryCache _cache;

public LoginController(MyContext context, IMemoryCache cache)
{
    _context = context;
    _cache = cache;
}
        public class LoginIntento
        {
            public int Intentos { get; set; } = 0;
            public DateTime? BloqueadoHasta { get; set; }
        }

        public IActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    // Redirigir según el rol del usuario
            //    if (User.IsInRole("administrador"))
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", "Instructor");
            //    }
            //}

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Nombre_usuario, string Contraseña)
        {
            var cacheKey = $"login-{Nombre_usuario}";

            // Obtener estado de intento desde cache
            var estado = _cache.Get<LoginIntento>(cacheKey) ?? new LoginIntento();

            // ¿Está bloqueado?
            if (estado.BloqueadoHasta.HasValue && estado.BloqueadoHasta > DateTime.Now)
            {
                var restante = estado.BloqueadoHasta.Value - DateTime.Now;
                ModelState.AddModelError("", $"Demasiados intentos. Espera {restante.Minutes} min y {restante.Seconds} seg.");
                return View("Index");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.NombreUsuario == Nombre_usuario);

            if (usuario == null || usuario.Password != Contraseña)
            {
                estado.Intentos++;

                if (estado.Intentos >= 3)
                {
                    estado.BloqueadoHasta = DateTime.Now.AddSeconds(45);
                    _cache.Set(cacheKey, estado, estado.BloqueadoHasta.Value);
                    ModelState.AddModelError("", "Usuario bloqueado por 45 segundos.");
                }
                else
                {
                    _cache.Set(cacheKey, estado, TimeSpan.FromMinutes(5));
                    ModelState.AddModelError("", $"Credenciales incorrectas. Intentos restantes: {3 - estado.Intentos}");
                }

                return View("Index");
            }

            // Autenticación exitosa: eliminar entrada del cache
            _cache.Remove(cacheKey);
            await SetUserCookie(usuario);

            if (usuario.Rol == Helluz.Dto.Roles.administrador)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Index", "Instructor");
        }

        private async Task SetUserCookie(Usuario usuario)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario!.NombreUsuario!),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol!.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
