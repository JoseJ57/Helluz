using Helluz.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Usuario
    {
        [Key]
        public int Id_usuario { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "El Nombre del Alumno no puede tener mas de 20 caracteres.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "El Nombre del Alumno solo puede contener letras.")]
        public string? Nombre_usuario { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "La contraseña no puede tener mas de 20 caracteres.")]
        public string? Password { get; set; }
        [Required]
        public bool Estado {  get; set; }
        [Required]
        public Roles Rol { get; set; }
        [Required]
        public string? Id_instructor { get; set; }
        [ForeignKey("Id_instructor")]
        public Instructor? Instructor { get; set; }
    }
}
