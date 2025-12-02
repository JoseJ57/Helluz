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
        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        public string? Nombre { get; set; }

        [Required]
        public float Costo { get; set; }

        [Required(ErrorMessage = "Los días por semana son obligatorios")]
        [Range(1, 5, ErrorMessage = "Debe ingresar entre 1 y 5 días por semana")]
        public int DiasPorSemana { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Debe ingresar entre 1 y 500")]
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
