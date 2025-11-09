using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Instructor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdInstructor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El carnet es obligatorio.")]
        [StringLength(20, ErrorMessage = "El carnet no puede tener más de 20 caracteres.")]
        public string Carnet { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
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

        [EmailAddress(ErrorMessage = "Correo inválido.")]
        public string? Correo { get; set; } // opcional

        [Required(ErrorMessage = "El celular es obligatorio.")]
        [Phone(ErrorMessage = "Número de celular inválido.")]
        public string Celular { get; set; } = null!;

        [Required]
        public bool Estado { get; set; } = true;

        // Relación opcional con Usuario
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // Relaciones
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
        public ICollection<AsistenciaInstructor> AsistenciaInstructors { get; set; } = new List<AsistenciaInstructor>();
    }
}
