using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class AsistenciaAlumno
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdAsistenciaAlumno { get; set; }
        [Required]
        public DateOnly Fecha { get; set; }
        [Required]
        public TimeOnly Hora { get; set; }
        [Required]
        public EstadoAsistencia Estado { get; set; }
        [Required]
        public int IdAlumno {  get; set; }
        [ForeignKey ("IdAlumno")]
        public Alumno? Alumno { get; set; }
    }
}
