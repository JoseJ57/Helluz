using System.ComponentModel.DataAnnotations;

namespace Helluz.Dto
{
    public enum EstadoInscripcion
    {
        [Display(Name = "Activa")]
        Activa = 0,

        [Display(Name = "Vencida")]
        Vencida = 1
    }
}
