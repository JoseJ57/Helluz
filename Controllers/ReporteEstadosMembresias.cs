using Helluz.Contexto;
using Helluz.Dto;
using Helluz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helluz.Controllers
{
    [Authorize]
    public class ReporteEstadosMembresiasController : Controller
    {
        private readonly MyContext _context;

        public ReporteEstadosMembresiasController(MyContext context)
        {
            _context = context;
        }

        // GET: ReporteEstadosMembresias/Index
        public async Task<IActionResult> Index(
            string busqueda,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            EstadoInscripcion? estado)
        {
            // Consulta base con todas las relaciones necesarias
            var query = _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .AsQueryable();

            // ===== FILTROS =====

            // 1. Filtro por búsqueda (Nombre, Apellido o Carnet)
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var searchTerm = busqueda.Trim().ToLower();
                query = query.Where(i =>
                    i.Alumno.Nombre.ToLower().Contains(searchTerm) ||
                    i.Alumno.Apellido.ToLower().Contains(searchTerm) ||
                    i.Alumno.Carnet.ToLower().Contains(searchTerm)
                );
            }

            // 2. Filtro por fecha desde
            if (fechaDesde.HasValue)
            {
                var fechaDesdeDateOnly = DateOnly.FromDateTime(fechaDesde.Value);
                query = query.Where(i => i.FechaInicio >= fechaDesdeDateOnly);
            }

            // 3. Filtro por fecha hasta
            if (fechaHasta.HasValue)
            {
                var fechaHastaDateOnly = DateOnly.FromDateTime(fechaHasta.Value);
                query = query.Where(i => i.FechaInicio <= fechaHastaDateOnly);
            }

            // 4. Filtro por estado
            if (estado.HasValue)
            {
                query = query.Where(i => i.Estado == estado.Value);
            }

            // ===== EJECUTAR CONSULTA =====
            var inscripciones = await query
                .OrderByDescending(i => i.FechaInicio)
                .ThenBy(i => i.Alumno.Apellido)
                .ToListAsync();

            // ===== ESTADÍSTICAS =====
            ViewBag.TotalInscripciones = inscripciones.Count;
            ViewBag.InscripcionesActivas = inscripciones.Count(i => i.Estado == EstadoInscripcion.Activa);
            ViewBag.InscripcionesVencidas = inscripciones.Count(i => i.Estado == EstadoInscripcion.Vencida);

            // Calcular ingresos totales
            ViewBag.IngresoTotal = inscripciones.Sum(i => i.Membresia?.Costo ?? 0);

            // ===== MANTENER VALORES DE FILTROS =====
            ViewBag.Busqueda = busqueda;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.EstadoSeleccionado = estado;

            return View(inscripciones);
        }

        // GET: ReporteEstadosMembresias/Exportar
        public async Task<IActionResult> Exportar(
            string busqueda,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            EstadoInscripcion? estado)
        {
            // Misma lógica de filtros que Index
            var query = _context.Inscripcions
                .Include(i => i.Alumno)
                .Include(i => i.Membresia)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var searchTerm = busqueda.Trim().ToLower();
                query = query.Where(i =>
                    i.Alumno.Nombre.ToLower().Contains(searchTerm) ||
                    i.Alumno.Apellido.ToLower().Contains(searchTerm) ||
                    i.Alumno.Carnet.ToLower().Contains(searchTerm)
                );
            }

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

            if (estado.HasValue)
            {
                query = query.Where(i => i.Estado == estado.Value);
            }

            var inscripciones = await query
                .OrderByDescending(i => i.FechaInicio)
                .ToListAsync();

            // Generar CSV
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Carnet,Nombre,Apellido,Membresía,Precio,Fecha Inicio,Fecha Fin,Estado");

            foreach (var i in inscripciones)
            {
                csv.AppendLine($"{i.Alumno.Carnet},{i.Alumno.Nombre},{i.Alumno.Apellido}," +
                              $"{i.Membresia?.Nombre},{i.Membresia?.Costo:C}," +
                              $"{i.FechaInicio:dd/MM/yyyy},{i.FechaFin:dd/MM/yyyy},{i.Estado}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"Reporte_Membresias_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }
    }
}