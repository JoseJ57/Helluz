using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class AsistenciaInstructor
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdAsistenciaInstructor { get; set; }
        [Required]
        public DateOnly Fecha { get; set; }
        [Required]
        public TimeOnly Hora { get; set; }
        [Required]
        public EstadoAsistencia Estado { get; set; }
        [Required]
        public int IdInstructor { get; set; }
        [ForeignKey("IdInstructor")]
        public Instructor? Instructor{ get; set; }

    }
}
