using Helluz.Contexto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Helluz.Contexto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [Authorize(Roles = "administrador")]
    public class ReportesFinancierosController : Controller
    {
        private readonly MyContext _context;

        public ReportesFinancierosController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateOnly? fechaInicio, DateOnly? fechaFin)
        {
            var query = _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(i => i.FechaInicio >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(i => i.FechaInicio <= fechaFin.Value);

            // 📊 Consulta sin modelo intermedio
            var reporte = await query
                .Select(i => new
                {
                    NombreCompleto = i.Alumno.Nombre + " " + i.Alumno.Apellido,
                    Tipo = i.Membresia.EsPromocion ? "Promoción" : "Membresía",
                    NombreTipo = i.Membresia.Nombre,
                    Costo = i.Membresia.Costo,
                    FechaInicio = i.FechaInicio
                })
                .OrderBy(r => r.FechaInicio)
                .ToListAsync();

            var total = reporte.Sum(r => (decimal)r.Costo);

            // Pasamos los datos a la vista mediante ViewBag
            ViewBag.Total = total;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View(reporte);
        }
    }
}
