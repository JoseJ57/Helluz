using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helluz.Migrations
{
    /// <inheritdoc />
    public partial class Disciplinas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Horarios_Disciplinas_IdDicsiplina",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Horarios_IdDicsiplina",
                table: "Horarios");

            migrationBuilder.DropColumn(
                name: "IdDicsiplina",
                table: "Horarios");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_IdDisciplina",
                table: "Horarios",
                column: "IdDisciplina");

            migrationBuilder.AddForeignKey(
                name: "FK_Horarios_Disciplinas_IdDisciplina",
                table: "Horarios",
                column: "IdDisciplina",
                principalTable: "Disciplinas",
                principalColumn: "IdDisciplina",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Horarios_Disciplinas_IdDisciplina",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Horarios_IdDisciplina",
                table: "Horarios");

            migrationBuilder.AddColumn<int>(
                name: "IdDicsiplina",
                table: "Horarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_IdDicsiplina",
                table: "Horarios",
                column: "IdDicsiplina");

            migrationBuilder.AddForeignKey(
                name: "FK_Horarios_Disciplinas_IdDicsiplina",
                table: "Horarios",
                column: "IdDicsiplina",
                principalTable: "Disciplinas",
                principalColumn: "IdDisciplina");
        }
    }
}
