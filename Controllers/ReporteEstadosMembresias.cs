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
    [Authorize]
    public class ReporteEstadosMembresias : Controller
    {
        private readonly MyContext _context;
        public ReporteEstadosMembresias(MyContext context)
        {
            _context = context;
        }
        // GET: ReporteEstadosMembresias
        public ActionResult Index()
        {
            return View();
        }
        
        // Agregar este método a tu InscripcionesController existente

        // GET: Inscripciones/Reporte
        public async Task<IActionResult> Reporte(string busqueda, DateTime? fechaDesde, DateTime? fechaHasta, Helluz.Dto.EstadoInscripcion? estado)
        {
            var query = _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .AsQueryable();

            // Filtro de búsqueda por nombre, apellido o carnet del alumno
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var searchTerm = busqueda.Trim().ToLower();
                query = query.Where(i =>
                    i.Alumno.Nombre.ToLower().Contains(searchTerm) ||
                    i.Alumno.Apellido.ToLower().Contains(searchTerm) ||
                    i.Alumno.Carnet.ToLower().Contains(searchTerm)
                );
            }

            // Filtro de fechas - inscripciones entre fechas
            if (fechaDesde.HasValue)
            {
                var fechaDesdeDateOnly = DateOnly.FromDateTime(fechaDesde.Value);
                query = query.Where(i => i.FechaInicio >= fechaDesdeDateOnly);
            }

            if (fechaHasta.HasValue)
            {
                var fechaHastaDateOnly = DateOnly.FromDateTime(fechaHasta.Value);
                query = query.Where(i => i.FechaInicio <= fechaHastaDateOnly);
            }

            // Filtro por estado de inscripción
            if (estado.HasValue)
            {
                query = query.Where(i => i.Estado == estado.Value);
            }

            var inscripciones = await query
                .OrderByDescending(i => i.FechaInicio)
                .ToListAsync();

            // Pasar los filtros actuales a la vista para mantenerlos en el formulario
            ViewBag.Busqueda = busqueda;
            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;
            ViewBag.EstadoFiltro = estado;

            // Estadísticas para el reporte
            ViewBag.TotalInscripciones = inscripciones.Count;
            ViewBag.InscripcionesActivas = inscripciones.Count(i => i.Estado == Dto.EstadoInscripcion.activa);
            ViewBag.InscripcionesVencidas = inscripciones.Count(i => i.Estado == Dto.EstadoInscripcion.venciada);

            return View(inscripciones);
        }
    }
}
