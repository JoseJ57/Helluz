using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class FaltaAlumno
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_faltaa { get; set; }
        [Required]
        public DateOnly Fecha { get; set; }
        [Required]
        public int Id_alumno { get; set; }
        [ForeignKey("Id_alumno")]
        public Alumno? Alumno { get; set; }
        [Required]
        public int Id_horario { get; set; }
        [ForeignKey("Id_horario")]
        public Horario? Horario  { get; set; }
    }
}
