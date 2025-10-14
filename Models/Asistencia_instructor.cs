using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Asistencia_instructor
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_asistencia { get; set; }
        [Required]
        public DateOnly Fecha { get; set; }
        [Required]
        public TimeOnly Hora { get; set; }
        [Required]
        public int Id_instructor { get; set; }
        [ForeignKey("Id_instructor")]
        public Instructor? instructor { get; set; }
        [Required]
        public int Id_horario { get; set; }
        [ForeignKey("Id_horario")]
        public Horario? Horario { get; set; }

    }
}
