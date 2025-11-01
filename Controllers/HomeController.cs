using System.Diagnostics;
using Helluz.Models;
using Microsoft.AspNetCore.Mvc;
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
    public class HomeController : Controller
    {
        private readonly MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalAlumnos = _context.Alumnos.Count(),
                TotalUsuarios = _context.Usuarios.Count(),
                TotalInstructores = _context.Instructors.Count()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
