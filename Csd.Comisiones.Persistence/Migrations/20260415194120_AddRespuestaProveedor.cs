using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRespuestaProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RespuestaProveedor",
                columns: table => new
                {
                    RespuestaProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime", nullable: true),
                    Aceptado = table.Column<bool>(type: "bit", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Vigente = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestaProveedor", x => x.RespuestaProveedorId);
                    table.ForeignKey(
                        name: "FK_RespuestaProveedor_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespuestaProveedor_Solicitud_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaProveedor_ProveedorId",
                table: "RespuestaProveedor",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaProveedor_SolicitudId",
                table: "RespuestaProveedor",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaProveedor_Token",
                table: "RespuestaProveedor",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespuestaProveedor");
        }
    }
}
