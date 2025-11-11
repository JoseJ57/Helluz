using Microsoft.EntityFrameworkCore;
using Helluz.Models;

namespace Helluz.Contexto
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        // tablas
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<AsistenciaAlumno> AsistenciaAlumnos { get; set; }
        public DbSet<AsistenciaInstructor> AsistenciaInstructor { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<Horario> Horarios { get; set; }
        public DbSet<Inscripcion> Inscripcions { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<TokenQr> TokenQrs { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}