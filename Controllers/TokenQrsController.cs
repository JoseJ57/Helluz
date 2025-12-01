using Helluz.Contexto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [Authorize(Roles = "administrador,instructor")]
    public class TokenQrsController : Controller
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment _env;

        public TokenQrsController(MyContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: TokenQrs
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("administrador") && !User.IsInRole("instructor"))
            {
                return Unauthorized();
            }

            var tokens = await _context.TokenQrs
                .OrderByDescending(t => t.FechaGeneracion)
                .ToListAsync();
            return View(tokens);
        }

        // GET: TokenQrs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tokenQr = await _context.TokenQrs
                .FirstOrDefaultAsync(m => m.IdToken == id);

            if (tokenQr == null) return NotFound();

            return View(tokenQr);
        }

        // GET: TokenQrs/ObtenerQr/5
        public IActionResult ObtenerQr(int id)
        {
            string qrPath = Path.Combine(_env.WebRootPath, "qr", $"qr_{id}.png");

            if (!System.IO.File.Exists(qrPath))
                return NotFound();

            var image = System.IO.File.OpenRead(qrPath);
            return File(image, "image/png");
        }

        // GET: TokenQrs/GenerarQr
        public async Task<IActionResult> GenerarQr()
        {
            // 1. Desactivar token activo anterior
            var tokenActivo = await _context.TokenQrs.FirstOrDefaultAsync(t => t.Estado == true);
            if (tokenActivo != null)
            {
                tokenActivo.Estado = false;
                _context.Update(tokenActivo);
            }

            // 2. Crear nuevo token
            var nuevoToken = new TokenQr
            {
                Token = Guid.NewGuid().ToString(),
                FechaGeneracion = DateOnly.FromDateTime(DateTime.Now),
                Estado = true
            };

            _context.Add(nuevoToken);
            await _context.SaveChangesAsync();

            // 3. Generar URL completa
            string? urlAsistencia = Url.Action(
                "RegistroAsistencia",
                "Asistencia",
                new { token = nuevoToken.Token },
                protocol: Request.Scheme
            );

            if (string.IsNullOrEmpty(urlAsistencia))
            {
                TempData["Error"] = "No se pudo generar la URL para el QR.";
                return RedirectToAction("Index");
            }

            // 4. Crear directorio para QR
            string qrDirectory = Path.Combine(_env.WebRootPath, "qr");
            if (!Directory.Exists(qrDirectory))
                Directory.CreateDirectory(qrDirectory);

            string qrPath = Path.Combine(qrDirectory, $"qr_{nuevoToken.IdToken}.png");

            // 5. Generar QR como PNG multiplataforma
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(urlAsistencia, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrData);
                byte[] qrBytes = qrCode.GetGraphic(20);

                await System.IO.File.WriteAllBytesAsync(qrPath, qrBytes);
            }

            TempData["Success"] = "QR generado correctamente.";
            return RedirectToAction("Index");
        }

        // GET: TokenQrs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tokenQr = await _context.TokenQrs.FindAsync(id);
            if (tokenQr == null) return NotFound();

            return View(tokenQr);
        }

        // POST: TokenQrs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdToken,Estado")] TokenQr tokenQr)
        {
            if (id != tokenQr.IdToken) return NotFound();

            var original = await _context.TokenQrs.AsNoTracking().FirstOrDefaultAsync(t => t.IdToken == id);
            if (original == null) return NotFound();

            tokenQr.Token = original.Token;
            tokenQr.FechaGeneracion = original.FechaGeneracion;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tokenQr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TokenQrs.Any(e => e.IdToken == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tokenQr);
        }

        // GET: TokenQrs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tokenQr = await _context.TokenQrs
                .FirstOrDefaultAsync(m => m.IdToken == id);
            if (tokenQr == null) return NotFound();

            return View(tokenQr);
        }

        // POST: TokenQrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tokenQr = await _context.TokenQrs.FindAsync(id);
            if (tokenQr != null)
            {
                _context.TokenQrs.Remove(tokenQr);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TokenQrExists(int id)
        {
            return _context.TokenQrs.Any(e => e.IdToken == id);
        }
    }
}
