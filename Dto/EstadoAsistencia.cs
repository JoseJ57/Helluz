using System.ComponentModel.DataAnnotations;

namespace Helluz.Dto
{
    public enum EstadoAsistencia
    {
        [Display(Name = "Presente")]
        presente = 0,

        [Display(Name = "Tarde")]
        tarde = 1,

        [Display(Name = "Falta")]
        falta = 2,

        [Display(Name = "Permiso")]
        permiso = 3,

        [Display(Name = "Otro horario")]
        otro = 4
    }
}
