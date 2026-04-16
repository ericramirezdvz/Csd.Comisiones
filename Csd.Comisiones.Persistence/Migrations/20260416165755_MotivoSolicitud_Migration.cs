using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MotivoSolicitud_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MotivoSolicitudId",
                table: "Solicitud",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MotivoSolicitud",
                columns: table => new
                {
                    MotivoSolicitudId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivoSolicitud", x => x.MotivoSolicitudId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_MotivoSolicitudId",
                table: "Solicitud",
                column: "MotivoSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_MotivoSolicitud_Nombre",
                table: "MotivoSolicitud",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitud_MotivoSolicitud_MotivoSolicitudId",
                table: "Solicitud",
                column: "MotivoSolicitudId",
                principalTable: "MotivoSolicitud",
                principalColumn: "MotivoSolicitudId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitud_MotivoSolicitud_MotivoSolicitudId",
                table: "Solicitud");

            migrationBuilder.DropTable(
                name: "MotivoSolicitud");

            migrationBuilder.DropIndex(
                name: "IX_Solicitud_MotivoSolicitudId",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "MotivoSolicitudId",
                table: "Solicitud");
        }
    }
}
