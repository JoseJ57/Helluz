using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Disciplina
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_disciplina { get; set; }
        [Required]
        public string? Nombre_disciplina { get; set; }
        public ICollection<Horario> Horarios { get; set; }= new List <Horario>(); 

    }
}
