using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helluz.Controllers
{
    [Authorize]
    public class ReporteFaltasController : Controller
    {
        // GET: ReporteFaltasController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReporteFaltasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReporteFaltasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReporteFaltasController/Create
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

        // GET: ReporteFaltasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReporteFaltasController/Edit/5
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

        // GET: ReporteFaltasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReporteFaltasController/Delete/5
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
