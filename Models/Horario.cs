using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class Horario
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_horario { get; set; }
        [Required]
        public TimeOnly Hora_inicio { set; get; }
        [Required]
        public TimeOnly Hora_fin { set; get; }
        [Required]
        public Dias Dia { set; get; }
        [Required]
        public int Id_disciplina { get; set; }
        [ForeignKey("Id_dicsiplina")]
        public Disciplina? Disciplina { get; set; }
        public ICollection<Horario> Horarios { get; set; }= new HashSet<Horario>();
        public int Id_instructor { get; set; }
        [ForeignKey("Id_instructor")]
        public Instructor? Instructor { get;set; }
        
    }
}
