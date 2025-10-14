using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helluz.Dto;

namespace Helluz.Models
{
    public class Inscripcion
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id_inscripcion { get; set; }
        [Required]
        public DateOnly Fecha_inicio { get; set; }
        [Required]
        public DateTime Fecha_fin { get; set; }
        [Required]
        public Metodos_pagos? Metodo_pago { get; set; }
        [Required]
        public int Nro_permisos { get; set; }
        [Required]
        public int Id_alumno { get; set; }
        [ForeignKey("Id_alumno")]
        public Alumno? Alumno { get; set; }
        [Required]
        public int Id_membresia { get; set; }
        [ForeignKey("Id_membresia")]
        public Membresia? Membresia { get; set; }
    }
}
