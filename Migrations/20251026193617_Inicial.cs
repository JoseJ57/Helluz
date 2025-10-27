using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helluz.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Edad",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Alumnos");

            migrationBuilder.RenameColumn(
                name: "Nro_emergencia",
                table: "Alumnos",
                newName: "NroEmergencia");

            migrationBuilder.RenameColumn(
                name: "Id_alumno",
                table: "Alumnos",
                newName: "IdAlumno");

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Alumnos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaNacimiento",
                table: "Alumnos",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "AsistenciaAlumnos",
                columns: table => new
                {
                    IdAsistenciaAlumno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenciaAlumnos", x => x.IdAsistenciaAlumno);
                    table.ForeignKey(
                        name: "FK_AsistenciaAlumnos_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Disciplinas",
                columns: table => new
                {
                    IdDisciplina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDisciplina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplinas", x => x.IdDisciplina);
                });

            migrationBuilder.CreateTable(
                name: "Membresias",
                columns: table => new
                {
                    IdMembresia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Costo = table.Column<float>(type: "real", nullable: false),
                    Nro_sesiones = table.Column<int>(type: "int", nullable: false),
                    FechaActivo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInactivo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membresias", x => x.IdMembresia);
                });

            migrationBuilder.CreateTable(
                name: "TokenQrs",
                columns: table => new
                {
                    IdToken = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaGeneracion = table.Column<DateOnly>(type: "date", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenQrs", x => x.IdToken);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Estados = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Inscripcions",
                columns: table => new
                {
                    IdInscripcion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false),
                    NroPermisos = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdMembresia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripcions", x => x.IdInscripcion);
                    table.ForeignKey(
                        name: "FK_Inscripcions_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripcions_Membresias_IdMembresia",
                        column: x => x.IdMembresia,
                        principalTable: "Membresias",
                        principalColumn: "IdMembresia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    IdInstructor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Carnet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Celular = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.IdInstructor);
                    table.ForeignKey(
                        name: "FK_Instructors_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    IdHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    IdDisciplina = table.Column<int>(type: "int", nullable: false),
                    IdDicsiplina = table.Column<int>(type: "int", nullable: true),
                    IdInstructor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.IdHorario);
                    table.ForeignKey(
                        name: "FK_Horarios_Disciplinas_IdDicsiplina",
                        column: x => x.IdDicsiplina,
                        principalTable: "Disciplinas",
                        principalColumn: "IdDisciplina");
                    table.ForeignKey(
                        name: "FK_Horarios_Instructors_IdInstructor",
                        column: x => x.IdInstructor,
                        principalTable: "Instructors",
                        principalColumn: "IdInstructor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlumnoHorarios",
                columns: table => new
                {
                    IdAlumnoHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdHorario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoHorarios", x => x.IdAlumnoHorario);
                    table.ForeignKey(
                        name: "FK_AlumnoHorarios_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoHorarios_Horarios_IdHorario",
                        column: x => x.IdHorario,
                        principalTable: "Horarios",
                        principalColumn: "IdHorario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AsistenciaInstructor",
                columns: table => new
                {
                    IdAsistenciaInstructor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IdInstructor = table.Column<int>(type: "int", nullable: false),
                    HorarioIdHorario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenciaInstructor", x => x.IdAsistenciaInstructor);
                    table.ForeignKey(
                        name: "FK_AsistenciaInstructor_Horarios_HorarioIdHorario",
                        column: x => x.HorarioIdHorario,
                        principalTable: "Horarios",
                        principalColumn: "IdHorario");
                    table.ForeignKey(
                        name: "FK_AsistenciaInstructor_Instructors_IdInstructor",
                        column: x => x.IdInstructor,
                        principalTable: "Instructors",
                        principalColumn: "IdInstructor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoHorarios_IdAlumno",
                table: "AlumnoHorarios",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoHorarios_IdHorario",
                table: "AlumnoHorarios",
                column: "IdHorario");

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciaAlumnos_IdAlumno",
                table: "AsistenciaAlumnos",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciaInstructor_HorarioIdHorario",
                table: "AsistenciaInstructor",
                column: "HorarioIdHorario");

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciaInstructor_IdInstructor",
                table: "AsistenciaInstructor",
                column: "IdInstructor");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_IdDicsiplina",
                table: "Horarios",
                column: "IdDicsiplina");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_IdInstructor",
                table: "Horarios",
                column: "IdInstructor");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripcions_IdAlumno",
                table: "Inscripcions",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripcions_IdMembresia",
                table: "Inscripcions",
                column: "IdMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_UsuarioId",
                table: "Instructors",
                column: "UsuarioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoHorarios");

            migrationBuilder.DropTable(
                name: "AsistenciaAlumnos");

            migrationBuilder.DropTable(
                name: "AsistenciaInstructor");

            migrationBuilder.DropTable(
                name: "Inscripcions");

            migrationBuilder.DropTable(
                name: "TokenQrs");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropTable(
                name: "Membresias");

            migrationBuilder.DropTable(
                name: "Disciplinas");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Alumnos");

            migrationBuilder.RenameColumn(
                name: "NroEmergencia",
                table: "Alumnos",
                newName: "Nro_emergencia");

            migrationBuilder.RenameColumn(
                name: "IdAlumno",
                table: "Alumnos",
                newName: "Id_alumno");

            migrationBuilder.AddColumn<int>(
                name: "Edad",
                table: "Alumnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Alumnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
