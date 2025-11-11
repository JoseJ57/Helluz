using Helluz.Contexto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace Helluz.Controllers
{
    [Authorize]
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
        // En el método Index de TokenQrs, verifica que el nuevo token aparezca
        public async Task<IActionResult> Index()
        {
            var tokens = await _context.TokenQrs
                .OrderByDescending(t => t.FechaGeneracion)
                .ToListAsync();
            return View(tokens);
        }

        // GET: TokenQrs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tokenQr = await _context.TokenQrs
                .FirstOrDefaultAsync(m => m.IdToken == id);
            if (tokenQr == null)
            {
                return NotFound();
            }

            return View(tokenQr);
        }
        public IActionResult ObtenerQr(int id)
        {
            string qrPath = Path.Combine(_env.WebRootPath, "qr", $"qr_{id}.png");

            if (!System.IO.File.Exists(qrPath))
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(qrPath);
            return File(image, "image/png");
        }
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

            // 3. ⭐ GENERAR URL COMPLETA (esto faltaba)
            string urlAsistencia = Url.Action(
                "RegistrarAsistencia",
                "Asistencia",
                new { token = nuevoToken.Token },
                protocol: Request.Scheme
            );

            // DEBUG: Ver qué URL se genera
            Console.WriteLine($"URL en QR: {urlAsistencia}");

            // 4. Crear directorio para QR
            string qrDirectory = Path.Combine(_env.WebRootPath, "qr");
            if (!Directory.Exists(qrDirectory))
                Directory.CreateDirectory(qrDirectory);

            string qrPath = Path.Combine(qrDirectory, $"qr_{nuevoToken.IdToken}.png");

            // 5. Generar QR con la URL completa
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(urlAsistencia, QRCodeGenerator.ECCLevel.Q); // ← Cambio aquí
                var qrCode = new QRCode(qrData);
                using (var bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(qrPath, ImageFormat.Png);
                }
            }

            return RedirectToAction("Index");
        }

        // GET: TokenQrs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tokenQr = await _context.TokenQrs.FindAsync(id);
            if (tokenQr == null)
            {
                return NotFound();
            }
            return View(tokenQr);
        }

        // POST: TokenQrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdToken,Estado")] TokenQr tokenQr)
        {
            if (id != tokenQr.IdToken)
                return NotFound();

            var original = await _context.TokenQrs.AsNoTracking().FirstOrDefaultAsync(t => t.IdToken == id);
            if (original == null)
                return NotFound();

            // Mantener valores originales
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
                    if (!_context.TokenQrs.Any(e => e.IdToken == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tokenQr);
        }


        // GET: TokenQrs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tokenQr = await _context.TokenQrs
                .FirstOrDefaultAsync(m => m.IdToken == id);
            if (tokenQr == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TokenQrExists(int id)
        {
            return _context.TokenQrs.Any(e => e.IdToken == id);
        }
    }
}
