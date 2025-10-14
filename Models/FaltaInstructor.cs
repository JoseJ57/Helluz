using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Helluz.Models
{
    public class FaltaInstructor
    {
        [Key]
        public int Id_falta { get; set; }
        [Required]
        public DateOnly Fecha {  get; set; }
        [Required]
        public Tipos Tipo { get; set; }
        [Required]
        public int Id_instructor { get; set; }
        [ForeignKey("Id_instructor")]
        public Instructor? Instructor { get; set; }
        [Required]
        public int Id_horario { get; set; }
        [ForeignKey("Id_horario")]
        public Horario? Horario { get; set; }
    }
}
