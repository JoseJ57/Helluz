using Microsoft.EntityFrameworkCore;
using Helluz.Models;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options): base(options) 
    {     
    }

    // tablas
    public DbSet<Alumno> Alumnos { get; set; }

}
