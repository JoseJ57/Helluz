using Microsoft.AspNetCore.Mvc;

namespace Helluz.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
