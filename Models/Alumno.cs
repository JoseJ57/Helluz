using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Alumno
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdAlumno { get; set; }
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
        public string? Celular { get; set; }
        [Required]
        public string? NroEmergencia { get; set; }
        [EmailAddress(ErrorMessage ="El correo no es valido")]
        public string? Correo { get; set; }
        [Required]
        public bool Estado { get; set; }

        public ICollection<Inscripcion> Inscripciones {  get; set; }=new List<Inscripcion>();
        public ICollection<AsistenciaAlumno> Asistencia_Alumnos {  get; set; }=new List<AsistenciaAlumno>();

    }
}
