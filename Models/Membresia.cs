using Helluz.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Membresia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMembresia { get; set; }

        [Required]
        public string? Nombre { get; set; }

        [Required]
        public float Costo { get; set; }

        [Required]
        public int DiasPorSemana { get; set; }

        [Required]
        public int Duracion { get; set; }

        [Required]
        public UnidadTiempo UnidadTiempo { get; set; }

        public DateTime? FechaActivo { get; set; }
        public DateTime? FechaInactivo { get; set; }

        [Required]
        public bool EsPromocion { get; set; }

        [Required]
        public bool Estado { get; set; }

        public ICollection<Inscripcion> Inscripcion { get; set; } = new List<Inscripcion>();
    }
}
