using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csd.Comisiones.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ciudad",
                columns: table => new
                {
                    CiudadId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_Ciudad", x => x.CiudadId);
                });

            migrationBuilder.CreateTable(
                name: "Empleado",
                columns: table => new
                {
                    EmpleadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroEmpleado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleado", x => x.EmpleadoId);
                });

            migrationBuilder.CreateTable(
                name: "EstatusDetalle",
                columns: table => new
                {
                    EstatusDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstatusDetalle", x => x.EstatusDetalleId);
                });

            migrationBuilder.CreateTable(
                name: "EstatusSolicitud",
                columns: table => new
                {
                    EstatusSolicitudId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstatusSolicitud", x => x.EstatusSolicitudId);
                });

            migrationBuilder.CreateTable(
                name: "TiposComida",
                columns: table => new
                {
                    TipoComidaId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_TiposComida", x => x.TipoComidaId);
                });

            migrationBuilder.CreateTable(
                name: "TipsHabitacion",
                columns: table => new
                {
                    TipoHabitacionId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_TipsHabitacion", x => x.TipoHabitacionId);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoProveedor = table.Column<int>(type: "int", nullable: false),
                    CiudadId = table.Column<int>(type: "int", nullable: false),
                    ProporcionaHospedaje = table.Column<bool>(type: "bit", nullable: false),
                    ProporcionaAlimentos = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.ProveedorId);
                    table.ForeignKey(
                        name: "FK_Proveedor_Ciudad_CiudadId",
                        column: x => x.CiudadId,
                        principalTable: "Ciudad",
                        principalColumn: "CiudadId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    SolicitanteId = table.Column<int>(type: "int", nullable: false),
                    EstatusSolicitudId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitud", x => x.SolicitudId);
                    table.ForeignKey(
                        name: "FK_Solicitud_EstatusSolicitud_EstatusSolicitudId",
                        column: x => x.EstatusSolicitudId,
                        principalTable: "EstatusSolicitud",
                        principalColumn: "EstatusSolicitudId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudEmpleado",
                columns: table => new
                {
                    SolicitudEmpleadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudEmpleado", x => x.SolicitudEmpleadoId);
                    table.ForeignKey(
                        name: "FK_SolicitudEmpleado_Empleado_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudEmpleado_Solicitud_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudComida",
                columns: table => new
                {
                    SolicitudComidaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudEmpleadoId = table.Column<int>(type: "int", nullable: false),
                    TipoComidaId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    UbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EstatusDetalleId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudComida", x => x.SolicitudComidaId);
                    table.ForeignKey(
                        name: "FK_SolicitudComida_EstatusDetalle_EstatusDetalleId",
                        column: x => x.EstatusDetalleId,
                        principalTable: "EstatusDetalle",
                        principalColumn: "EstatusDetalleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudComida_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudComida_SolicitudEmpleado_SolicitudEmpleadoId",
                        column: x => x.SolicitudEmpleadoId,
                        principalTable: "SolicitudEmpleado",
                        principalColumn: "SolicitudEmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudComida_TiposComida_TipoComidaId",
                        column: x => x.TipoComidaId,
                        principalTable: "TiposComida",
                        principalColumn: "TipoComidaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudHotel",
                columns: table => new
                {
                    SolicitudHotelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudEmpleadoId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    TipoHabitacionId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EstatusDetalleId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudHotel", x => x.SolicitudHotelId);
                    table.ForeignKey(
                        name: "FK_SolicitudHotel_EstatusDetalle_EstatusDetalleId",
                        column: x => x.EstatusDetalleId,
                        principalTable: "EstatusDetalle",
                        principalColumn: "EstatusDetalleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudHotel_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudHotel_SolicitudEmpleado_SolicitudEmpleadoId",
                        column: x => x.SolicitudEmpleadoId,
                        principalTable: "SolicitudEmpleado",
                        principalColumn: "SolicitudEmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudHotel_TipsHabitacion_TipoHabitacionId",
                        column: x => x.TipoHabitacionId,
                        principalTable: "TipsHabitacion",
                        principalColumn: "TipoHabitacionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ciudad_Nombre",
                table: "Ciudad",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_AreaId",
                table: "Empleado",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_NumeroEmpleado",
                table: "Empleado",
                column: "NumeroEmpleado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstatusDetalle_Nombre",
                table: "EstatusDetalle",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_CiudadId",
                table: "Proveedor",
                column: "CiudadId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_Nombre_CiudadId",
                table: "Proveedor",
                columns: new[] { "Nombre", "CiudadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_EstatusSolicitudId",
                table: "Solicitud",
                column: "EstatusSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudComida_EstatusDetalleId",
                table: "SolicitudComida",
                column: "EstatusDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudComida_ProveedorId",
                table: "SolicitudComida",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudComida_SolicitudEmpleadoId",
                table: "SolicitudComida",
                column: "SolicitudEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudComida_TipoComidaId",
                table: "SolicitudComida",
                column: "TipoComidaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudEmpleado_EmpleadoId",
                table: "SolicitudEmpleado",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudEmpleado_SolicitudId",
                table: "SolicitudEmpleado",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudHotel_EstatusDetalleId",
                table: "SolicitudHotel",
                column: "EstatusDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudHotel_ProveedorId",
                table: "SolicitudHotel",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudHotel_SolicitudEmpleadoId",
                table: "SolicitudHotel",
                column: "SolicitudEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudHotel_TipoHabitacionId",
                table: "SolicitudHotel",
                column: "TipoHabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposComida_Nombre",
                table: "TiposComida",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipsHabitacion_Nombre",
                table: "TipsHabitacion",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudComida");

            migrationBuilder.DropTable(
                name: "SolicitudHotel");

            migrationBuilder.DropTable(
                name: "TiposComida");

            migrationBuilder.DropTable(
                name: "EstatusDetalle");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "SolicitudEmpleado");

            migrationBuilder.DropTable(
                name: "TipsHabitacion");

            migrationBuilder.DropTable(
                name: "Ciudad");

            migrationBuilder.DropTable(
                name: "Empleado");

            migrationBuilder.DropTable(
                name: "Solicitud");

            migrationBuilder.DropTable(
                name: "EstatusSolicitud");
        }
    }
}
