using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Membresia
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_membresia { get; set; }
        [Required]
        public string? Nombre { get; set; }
        [Required]
        public decimal Costo { get; set; }
        [Required]
        public int Nro_sesiones { get; set; }
        [Required]
        public DateTime Fecha_inicio { get; set; }
        [Required]
        public DateTime Fecha_fin { get; set; }
        [Required]
        public bool Estado { get; set; }
        public ICollection<Inscripcion> Inscripcion { get; set; } = new List<Inscripcion>();
    }
}