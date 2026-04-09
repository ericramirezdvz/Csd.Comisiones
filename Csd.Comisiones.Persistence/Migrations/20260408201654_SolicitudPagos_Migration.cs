using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SolicitudPagos_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UbicacionId",
                table: "SolicitudComida",
                newName: "UbicacionAlimentoId");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPago",
                table: "SolicitudEmpleado",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoAsignacion",
                table: "SolicitudEmpleado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UbicacionAlimento",
                columns: table => new
                {
                    UbicacionAlimentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UbicacionAlimento", x => x.UbicacionAlimentoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudComida_UbicacionAlimentoId",
                table: "SolicitudComida",
                column: "UbicacionAlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionAlimento_Nombre",
                table: "UbicacionAlimento",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudComida_UbicacionAlimento_UbicacionAlimentoId",
                table: "SolicitudComida",
                column: "UbicacionAlimentoId",
                principalTable: "UbicacionAlimento",
                principalColumn: "UbicacionAlimentoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudComida_UbicacionAlimento_UbicacionAlimentoId",
                table: "SolicitudComida");

            migrationBuilder.DropTable(
                name: "UbicacionAlimento");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudComida_UbicacionAlimentoId",
                table: "SolicitudComida");

            migrationBuilder.DropColumn(
                name: "MontoPago",
                table: "SolicitudEmpleado");

            migrationBuilder.DropColumn(
                name: "TipoAsignacion",
                table: "SolicitudEmpleado");

            migrationBuilder.RenameColumn(
                name: "UbicacionAlimentoId",
                table: "SolicitudComida",
                newName: "UbicacionId");
        }
    }
}
