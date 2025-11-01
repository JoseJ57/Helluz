using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helluz.Migrations
{
    /// <inheritdoc />
    public partial class ESPROMO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsPromocion",
                table: "Membresias",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsPromocion",
                table: "Membresias");
        }
    }
}
