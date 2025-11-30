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
        [StringLength(20, ErrorMessage = "El nombre no puede tener más de 20 caracteres.")]
        public string? NombreDisciplina { get; set; }
        [StringLength(120, ErrorMessage = "La descripción no puede tener más de 120 caracteres.")]
        public string? Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Eliminar o ignorar la colección de horarios
        //[NotMapped]
        //public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
    }

}
