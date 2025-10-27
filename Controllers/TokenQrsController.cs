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
    public class TokenQrsController : Controller
    {
        private readonly MyContext _context;

        public TokenQrsController(MyContext context)
        {
            _context = context;
        }

        // GET: TokenQrs
        public async Task<IActionResult> Index()
        {
            return View(await _context.TokenQrs.ToListAsync());
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

        // GET: TokenQrs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TokenQrs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdToken,Token,FechaGeneracion,Estado")] TokenQr tokenQr)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tokenQr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tokenQr);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdToken,Token,FechaGeneracion,Estado")] TokenQr tokenQr)
        {
            if (id != tokenQr.IdToken)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tokenQr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TokenQrExists(tokenQr.IdToken))
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
