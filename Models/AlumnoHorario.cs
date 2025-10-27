using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class AlumnoHorario
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdAlumnoHorario{ get; set; }
        [Required]
        public int IdAlumno { get; set; }
        [ForeignKey("IdAlumno")]
        public Alumno? Alumno { get; set; }
        [Required]
        public int IdHorario { get; set; }
        [ForeignKey("IdHorario")]
        public Horario? Horario { get; set; }

    }
}
