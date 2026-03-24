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
                name: "Area",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                });

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
                name: "Obra",
                columns: table => new
                {
                    ObraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obra", x => x.ObraId);
                });

            migrationBuilder.CreateTable(
                name: "TipoComida",
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
                    table.PrimaryKey("PK_TipoComida", x => x.TipoComidaId);
                });

            migrationBuilder.CreateTable(
                name: "TipoHabitacion",
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
                    table.PrimaryKey("PK_TipoHabitacion", x => x.TipoHabitacionId);
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
                name: "Autorizador",
                columns: table => new
                {
                    AutorizadorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    EsAlterno = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autorizador", x => x.AutorizadorId);
                    table.ForeignKey(
                        name: "FK_Autorizador_Empleado_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Autorizador_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "ObraId",
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
                    ObraId = table.Column<int>(type: "int", nullable: false),
                    SolicitanteId = table.Column<int>(type: "int", nullable: false),
                    EstatusSolicitudId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitud", x => x.SolicitudId);
                    table.ForeignKey(
                        name: "FK_Solicitud_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitud_EstatusSolicitud_EstatusSolicitudId",
                        column: x => x.EstatusSolicitudId,
                        principalTable: "EstatusSolicitud",
                        principalColumn: "EstatusSolicitudId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitud_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "ObraId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAutorizacion",
                columns: table => new
                {
                    SolicitudAutorizacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    AutorizadorId = table.Column<int>(type: "int", nullable: false),
                    EstatusAutorizacionId = table.Column<int>(type: "int", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comentarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAutorizacion", x => x.SolicitudAutorizacionId);
                    table.ForeignKey(
                        name: "FK_SolicitudAutorizacion_Autorizador_AutorizadorId",
                        column: x => x.AutorizadorId,
                        principalTable: "Autorizador",
                        principalColumn: "AutorizadorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudAutorizacion_Solicitud_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudId",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudEmpleado_Solicitud_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudSeguimiento",
                columns: table => new
                {
                    SolicitudSeguimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    EstatusSolicitudId = table.Column<int>(type: "int", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudSeguimiento", x => x.SolicitudSeguimientoId);
                    table.ForeignKey(
                        name: "FK_SolicitudSeguimiento_EstatusSolicitud_EstatusSolicitudId",
                        column: x => x.EstatusSolicitudId,
                        principalTable: "EstatusSolicitud",
                        principalColumn: "EstatusSolicitudId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudSeguimiento_Solicitud_SolicitudId",
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudComida_TipoComida_TipoComidaId",
                        column: x => x.TipoComidaId,
                        principalTable: "TipoComida",
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudHotel_TipoHabitacion_TipoHabitacionId",
                        column: x => x.TipoHabitacionId,
                        principalTable: "TipoHabitacion",
                        principalColumn: "TipoHabitacionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_Nombre",
                table: "Area",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Autorizador_EmpleadoId",
                table: "Autorizador",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Autorizador_ObraId",
                table: "Autorizador",
                column: "ObraId");

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
                name: "IX_Obra_Nombre",
                table: "Obra",
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
                name: "IX_Solicitud_AreaId",
                table: "Solicitud",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_EstatusSolicitudId",
                table: "Solicitud",
                column: "EstatusSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_ObraId",
                table: "Solicitud",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudAutorizacion_AutorizadorId",
                table: "SolicitudAutorizacion",
                column: "AutorizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudAutorizacion_SolicitudId",
                table: "SolicitudAutorizacion",
                column: "SolicitudId");

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
                name: "IX_SolicitudSeguimiento_EstatusSolicitudId",
                table: "SolicitudSeguimiento",
                column: "EstatusSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudSeguimiento_SolicitudId",
                table: "SolicitudSeguimiento",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoComida_Nombre",
                table: "TipoComida",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipoHabitacion_Nombre",
                table: "TipoHabitacion",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudAutorizacion");

            migrationBuilder.DropTable(
                name: "SolicitudComida");

            migrationBuilder.DropTable(
                name: "SolicitudHotel");

            migrationBuilder.DropTable(
                name: "SolicitudSeguimiento");

            migrationBuilder.DropTable(
                name: "Autorizador");

            migrationBuilder.DropTable(
                name: "TipoComida");

            migrationBuilder.DropTable(
                name: "EstatusDetalle");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "SolicitudEmpleado");

            migrationBuilder.DropTable(
                name: "TipoHabitacion");

            migrationBuilder.DropTable(
                name: "Ciudad");

            migrationBuilder.DropTable(
                name: "Empleado");

            migrationBuilder.DropTable(
                name: "Solicitud");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "EstatusSolicitud");

            migrationBuilder.DropTable(
                name: "Obra");
        }
    }
}
