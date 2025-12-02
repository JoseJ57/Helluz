using Helluz.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "El Nombre no puede tener mas de 20 caracteres.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "El Nombre solo puede contener letras.")]
        public string? NombreUsuario { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 12 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-_=+\[\]{};:,.?])[^ ]{8,12}$",
    ErrorMessage = "La contraseña debe tener entre 8 y 12 caracteres e incluir mayúscula, minúscula, número y símbolo.")]
        public string? Password { get; set; }

        public bool Estado {  get; set; }
        [Required]
        public Roles Rol { get; set; }
        public Instructor? Instructor { get; set; }
    }
}
