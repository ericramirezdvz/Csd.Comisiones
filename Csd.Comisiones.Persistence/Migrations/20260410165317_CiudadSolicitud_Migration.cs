using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CiudadSolicitud_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CiudadId",
                table: "Solicitud",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_CiudadId",
                table: "Solicitud",
                column: "CiudadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitud_Ciudad_CiudadId",
                table: "Solicitud",
                column: "CiudadId",
                principalTable: "Ciudad",
                principalColumn: "CiudadId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitud_Ciudad_CiudadId",
                table: "Solicitud");

            migrationBuilder.DropIndex(
                name: "IX_Solicitud_CiudadId",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "CiudadId",
                table: "Solicitud");
        }
    }
}
