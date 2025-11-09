using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Disciplina
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDisciplina { get; set; }

        [Required]
        public string? NombreDisciplina { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Eliminar o ignorar la colección de horarios
        //[NotMapped]
        //public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
    }

}
