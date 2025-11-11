using System;
using System.ComponentModel.DataAnnotations;

namespace Helluz.Validations
{
    public class FechaNoFuturaAttribute : ValidationAttribute
    {
        public FechaNoFuturaAttribute()
        {
            ErrorMessage = "La fecha de nacimiento no puede ser mayor que la fecha actual.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // Deja que [Required] maneje el campo vacío

            if (value is DateOnly fechaNacimiento)
            {
                return fechaNacimiento <= DateOnly.FromDateTime(DateTime.Now);
            }

            return false;
        }
    }
}
