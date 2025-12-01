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
    [AllowAnonymous] // Permite acceso sin autenticación a este controlador
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
            // Si ya está autenticado, redirigir a Home
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken] // ⭐ Seguridad adicional
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

            return RedirectToAction("Index", "Home");
        }

        private async Task SetUserCookie(Usuario usuario)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario!.NombreUsuario!),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // ⭐ Configuración importante
                IsPersistent = false, // No recordar sesión al cerrar navegador
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }

        // ⭐ CAMBIOS CRÍTICOS EN LOGOUT
        [HttpPost] // Debe ser POST, no GET
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Cerrar sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar sesión
            HttpContext.Session.Clear();

            // ⭐ Headers para prevenir caché
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index", "Login");
        }

        // ⭐ Método auxiliar para verificar sesión (opcional)
        [HttpGet]
        public IActionResult CheckSession()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Ok();
            }
            return Unauthorized();
        }
    }
}