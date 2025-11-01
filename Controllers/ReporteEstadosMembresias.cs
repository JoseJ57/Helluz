using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helluz.Controllers
{
    public class ReporteEstadosMembresias : Controller
    {
        // GET: ReporteEstadosMembresias
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReporteEstadosMembresias/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReporteEstadosMembresias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReporteEstadosMembresias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReporteEstadosMembresias/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReporteEstadosMembresias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReporteEstadosMembresias/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReporteEstadosMembresias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
