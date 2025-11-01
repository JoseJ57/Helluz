using Helluz.Models;
using Microsoft.EntityFrameworkCore;
using Helluz.Contexto;
public class MembresiaService
{
    private readonly MyContext _context;

    public MembresiaService(MyContext context)
    {
        _context = context;
    }

    public async Task ActualizarEstadosPromociones()
    {
        var promociones = await _context.Membresias
            .Where(m => m.EsPromocion && m.FechaActivo.HasValue && m.FechaInactivo.HasValue)
            .ToListAsync();

        var hoy = DateTime.Now;

        foreach (var promo in promociones)
        {
            bool debeEstarActiva = EstaEnRangoFechas(
                hoy,
                promo.FechaActivo.Value,
                promo.FechaInactivo.Value
            );

            if (promo.Estado != debeEstarActiva)
            {
                promo.Estado = debeEstarActiva;
            }
        }

        await _context.SaveChangesAsync();
    }

    private bool EstaEnRangoFechas(DateTime fechaActual, DateTime fechaInicio, DateTime fechaFin)
    {
        var actualMesDia = new DateTime(2000, fechaActual.Month, fechaActual.Day);
        var inicioMesDia = new DateTime(2000, fechaInicio.Month, fechaInicio.Day);
        var finMesDia = new DateTime(2000, fechaFin.Month, fechaFin.Day);

        if (inicioMesDia <= finMesDia)
        {
            return actualMesDia >= inicioMesDia && actualMesDia <= finMesDia;
        }
        else
        {
            return actualMesDia >= inicioMesDia || actualMesDia <= finMesDia;
        }
    }

    public bool ValidarEstadoPromocion(Membresia membresia)
    {
        if (!membresia.EsPromocion) return membresia.Estado;

        if (!membresia.FechaActivo.HasValue || !membresia.FechaInactivo.HasValue)
            return membresia.Estado;

        return EstaEnRangoFechas(DateTime.Now, membresia.FechaActivo.Value, membresia.FechaInactivo.Value);
    }
}