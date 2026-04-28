using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExternosTipoPago_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudEmpleado_Empleado_EmpleadoId",
                table: "SolicitudEmpleado");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "SolicitudEmpleado",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "EsExterno",
                table: "SolicitudEmpleado",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NombreExterno",
                table: "SolicitudEmpleado",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPago",
                table: "SolicitudEmpleado",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudEmpleado_Empleado_EmpleadoId",
                table: "SolicitudEmpleado",
                column: "EmpleadoId",
                principalTable: "Empleado",
                principalColumn: "EmpleadoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudEmpleado_Empleado_EmpleadoId",
                table: "SolicitudEmpleado");

            migrationBuilder.DropColumn(
                name: "EsExterno",
                table: "SolicitudEmpleado");

            migrationBuilder.DropColumn(
                name: "NombreExterno",
                table: "SolicitudEmpleado");

            migrationBuilder.DropColumn(
                name: "TipoPago",
                table: "SolicitudEmpleado");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "SolicitudEmpleado",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudEmpleado_Empleado_EmpleadoId",
                table: "SolicitudEmpleado",
                column: "EmpleadoId",
                principalTable: "Empleado",
                principalColumn: "EmpleadoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
