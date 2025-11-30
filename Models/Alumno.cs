using Helluz.Validations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Alumno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAlumno { get; set; }

        [Required(ErrorMessage = "El nombre del alumno es obligatorio.")]
        [StringLength(30, ErrorMessage = "El nombre del alumno no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s'-]+$", ErrorMessage = "El nombre del alumno solo puede contener letras y espacios.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido del alumno es obligatorio.")]
        [StringLength(30, ErrorMessage = "El apellido del alumno no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s'-]+$", ErrorMessage = "El apellido del alumno solo puede contener letras y espacios.")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El carnet es obligatorio.")]
        [StringLength(10, ErrorMessage = "El carnet no puede tener más de 10 caracteres.")]
        public string? Carnet { get; set; }

        //[Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [FechaNoFutura]
        public DateOnly? FechaNacimiento { get; set; }

        [NotMapped]
        public int Edad
        {
            get
            {
                if (!FechaNacimiento.HasValue)
                    return 0;

                var fecha = FechaNacimiento.Value;
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                int edad = hoy.Year - fecha.Year;
                if (hoy < fecha.AddYears(edad))
                    edad--;
                return edad;
            }
        }

        [Required(ErrorMessage = "El celular es obligatorio.")]
        [Phone(ErrorMessage = "Número de celular inválido.")]
        public string? Celular { get; set; }

        //[Required(ErrorMessage = "El número de emergencia es obligatorio.")]
        [Phone(ErrorMessage = "Número de celular inválido.")]
        public string? NroEmergencia { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string? Correo { get; set; }

        [Required]
        public bool Estado { get; set; } = true;

        // Relaciones
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        public ICollection<AsistenciaAlumno> Asistencia_Alumnos { get; set; } = new List<AsistenciaAlumno>();
    }
}
