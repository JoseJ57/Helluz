using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helluz.Migrations
{
    /// <inheritdoc />
    public partial class AgregarControlDiasEnInscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ControlDias",
                table: "Inscripcions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlDias",
                table: "Inscripcions");
        }
    }
}
