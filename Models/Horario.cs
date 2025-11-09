using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class Horario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHorario { get; set; }

        [Required]
        public TimeOnly HoraInicio { get; set; }

        [Required]
        public TimeOnly HoraFin { get; set; }

        // Ahora cada día puede estar activo y tener su propia disciplina
        public bool Lunes { get; set; } = false;
        public int? IdDisciplinaLunes { get; set; }
        [ForeignKey("IdDisciplinaLunes")]
        public virtual Disciplina? DisciplinaLunes { get; set; }

        public bool Martes { get; set; } = false;
        public int? IdDisciplinaMartes { get; set; }
        [ForeignKey("IdDisciplinaMartes")]
        public virtual Disciplina? DisciplinaMartes { get; set; }

        public bool Miercoles { get; set; } = false;
        public int? IdDisciplinaMiercoles { get; set; }
        [ForeignKey("IdDisciplinaMiercoles")]
        public virtual Disciplina? DisciplinaMiercoles { get; set; }

        public bool Jueves { get; set; } = false;
        public int? IdDisciplinaJueves { get; set; }
        [ForeignKey("IdDisciplinaJueves")]
        public virtual Disciplina? DisciplinaJueves { get; set; }

        public bool Viernes { get; set; } = false;
        public int? IdDisciplinaViernes { get; set; }
        [ForeignKey("IdDisciplinaViernes")]
        public virtual Disciplina? DisciplinaViernes { get; set; }


        [Required]
        public int IdInstructor { get; set; }
        [ForeignKey("IdInstructor")]
        public Instructor? Instructor { get; set; }

        public ICollection<AlumnoHorario> AlumnoHorarios { get; set; } = new HashSet<AlumnoHorario>();
        public ICollection<AsistenciaInstructor> AsistenciaInstructors { get; set; } = new HashSet<AsistenciaInstructor>();
    }
}
