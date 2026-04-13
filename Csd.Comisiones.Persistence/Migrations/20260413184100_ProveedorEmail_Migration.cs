using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProveedorEmail_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Proveedor",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Proveedor");
        }
    }
}
