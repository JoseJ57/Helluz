using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Instructor
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_instructor { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "El Nombre del Alumno no puede tener mas de 20 caracteres.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "El Nombre del Alumno solo puede contener letras.")]
        public string? Nombre { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "El Apellido del Alumno no puede tener mas de 20 caracteres.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "El Apellido del Alumno solo puede contener letras.")]
        public string? Apellido { get; set; }
        [Required]
        public string? Carnet { get; set; }
        [Required]
        public DateOnly Edad {  get; set; }
        [Required]
        public int? Campo { get; set; }
        [Required]
        public int Celular { get; set; }
        
        public Usuario? Usuario { get; set; }
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();

    }
}
