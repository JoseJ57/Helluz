using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Alumno
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_alumno { get; set; }
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
        public int Edad { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Celular { get; set; }
        [Required]
        public string? Nro_emergencia { get; set; }
        [EmailAddress(ErrorMessage ="El correo no es valido")]
        public string? Correo { get; set; }
        public ICollection<Inscripcion> Inscripciones {  get; set; }=new List<Inscripcion>();
        public ICollection<FaltaAlumno> FaltaAlumnos {  get; set; }=new List<FaltaAlumno>();
        public ICollection<Asistencia_alumno> Asistencia_Alumnos {  get; set; }=new List<Asistencia_alumno>();

    }
}
