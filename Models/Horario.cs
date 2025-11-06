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
        public int IdHorario { get; set; }
        [Required]
        public TimeOnly HoraInicio { set; get; }
        [Required]
        public TimeOnly HoraFin { set; get; }
        [Required]
        public Dias DiaSemana { set; get; }
        [Required]
        public int IdDisciplina { get; set; }
        [ForeignKey("IdDisciplina")]
        public Disciplina? Disciplina { get; set; }
        public ICollection<AlumnoHorario> AlumnoHorarios { get; set; }= new HashSet<AlumnoHorario>();
        public ICollection<AsistenciaInstructor> AsistenciaInstructors { get; set; }= new HashSet<AsistenciaInstructor>();
        public int IdInstructor { get; set; }
        [ForeignKey("IdInstructor")]
        public Instructor? Instructor { get;set; }
        
    }
}
