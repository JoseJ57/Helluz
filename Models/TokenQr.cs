using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helluz.Models
{
    public class TokenQr
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IdToken { get; set; }
        [Required]
        public string? Token {  get; set; }
        [Required]
        public DateOnly FechaGeneracion {  get; set; }
        [Required]
        public bool Estado {  get; set; }
    }
}
