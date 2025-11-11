using System.ComponentModel.DataAnnotations;

namespace Helluz.Dto
{
    public enum MetodosPagos
    {
        [Display(Name = "Efectivo")]
        Efectivo = 0,

        [Display(Name = "Pago por QR")]
        Qr = 1,

        [Display(Name = "Transferencia Bancaria")]
        Transferencia = 2,

        [Display(Name = "Tarjeta de Crédito/Débito")]
        Tarjeta = 3
    }

}
