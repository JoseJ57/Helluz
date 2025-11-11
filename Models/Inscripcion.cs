using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class Inscripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public EstadoInscripcion Estado { get; set; }

        [Required]
        public int IdAlumno { get; set; }
        [ForeignKey("IdAlumno")]
        public Alumno? Alumno { get; set; }

        [Required]
        public int IdMembresia { get; set; }
        [ForeignKey("IdMembresia")]
        public Membresia? Membresia { get; set; }

        [Required]
        public int IdHorario { get; set; }
        [ForeignKey("IdHorario")]
        public Horario? Horario { get; set; }

        // 🔹 NUEVO CAMPO: Control de días asistidos por semana
        [Required]
        public int ControlDias { get; set; } = 0; // se inicializa automáticamente en 0
    }
}
