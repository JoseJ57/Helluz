using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Instructor
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdInstructor { get; set; }
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
        public DateOnly FechaNacimiento { get; set; }
        [NotMapped]
        public int Edad
        {
            get
            {
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                int edad = hoy.Year - FechaNacimiento.Year;
                if (hoy < FechaNacimiento.AddYears(edad))
                    edad--;
                return edad;
            }
        }
        [Required]
        [EmailAddress(ErrorMessage = "El correo no es valido")]
        public string? Correo { get; set; }
        [Required]
        public int Celular { get; set; }
        [Required]
        public bool Estado { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
        public ICollection<AsistenciaInstructor> AsistenciaInstructors { get; set; } = new List<AsistenciaInstructor>();

    }
}
