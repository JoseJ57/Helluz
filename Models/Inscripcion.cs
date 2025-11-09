using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class Inscripcion
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdInscripcion { get; set; }
        [Required]
        public DateOnly FechaInicio { get; set; }

        [Required]
        public DateOnly FechaFin { get; set; }

        [Required]
        public MetodosPagos MetodoPago { get; set; }
        [Required]
        public int NroPermisos { get; set; }
        [Required]
        public EstadoInscripcion Estado {  get; set; }
        [Required]
        public int IdAlumno { get; set; }
        [ForeignKey("IdAlumno")]
        public Alumno? Alumno { get; set; }
        [Required]
        public int IdMembresia { get; set; }
        [ForeignKey("IdMembresia")]
        public Membresia? Membresia { get; set; }
    }
}
