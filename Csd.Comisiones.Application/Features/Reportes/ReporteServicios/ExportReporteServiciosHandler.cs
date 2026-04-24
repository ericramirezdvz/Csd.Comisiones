using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Reportes.ReporteServicios
{
    public class ExportReporteServiciosHandler
    : IRequestHandler<ExportReporteServiciosQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;

        public ExportReporteServiciosHandler(
            IApplicationDbContext context,
            IExcelService excelService)
        {
            _context = context;
            _excelService = excelService;
        }

        public async Task<byte[]> Handle(
            ExportReporteServiciosQuery request,
            CancellationToken cancellationToken)
        {
            // 🏨 HOTELES (PROYECCIÓN DIRECTA)
            var hotelesQuery = _context.SolicitudHotel
                .AsNoTracking()
                .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

            if (request.FechaInicio.HasValue)
                hotelesQuery = hotelesQuery
                    .Where(h => h.SolicitudEmpleado.Solicitud.FechaInicio >= request.FechaInicio);

            if (request.FechaFin.HasValue)
                hotelesQuery = hotelesQuery
                    .Where(h => h.SolicitudEmpleado.Solicitud.FechaFin <= request.FechaFin);

            var hoteles = await hotelesQuery
            .Select(h => new
            {
                h,
                Noches = (h.FechaFin - h.FechaInicio).Days
            })
            .Select(x => new ReporteServiciosDto
            {
                AreaSolicitante = "STAFF",

                FechaSolicitud = x.h.SolicitudEmpleado.Solicitud.FechaInicio,
                Obra = x.h.SolicitudEmpleado.Solicitud.ObraId.ToString(),
                Motivo = x.h.SolicitudEmpleado.Solicitud.Comentarios,

                ClaseValoracion = "Servicio",
                Material = "HOSPEDAJE",
                Descripcion = "HOSPEDAJE",

                Cantidad = x.Noches,

                UM = "SER",
                Folio = x.h.SolicitudEmpleado.Solicitud.Folio,

                FechaRealServicio = x.h.FechaInicio,

                MontoSinIVA = x.Noches * x.h.PrecioUnitario,
                IVA = (x.Noches * x.h.PrecioUnitario) * 0.16m,
                Monto = (x.Noches * x.h.PrecioUnitario) * 1.16m,

                Moneda = "MXN",

                Proveedor = x.h.Proveedor.Nombre,

                Fecha = x.h.FechaInicio,
                Alcance = x.Noches + " NOCHES"
            })
            .ToListAsync(cancellationToken);

            // 🍽️ COMIDAS (PROYECCIÓN DIRECTA)
            var comidasQuery = _context.SolicitudComida
                .AsNoTracking()
                .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

            if (request.FechaInicio.HasValue)
                comidasQuery = comidasQuery
                    .Where(c => c.SolicitudEmpleado.Solicitud.FechaInicio >= request.FechaInicio);

            if (request.FechaFin.HasValue)
                comidasQuery = comidasQuery
                    .Where(c => c.SolicitudEmpleado.Solicitud.FechaFin <= request.FechaFin);

            var comidas = await comidasQuery
            .Select(c => new ReporteServiciosDto
            {
                AreaSolicitante = "STAFF",

                FechaSolicitud = c.SolicitudEmpleado.Solicitud.FechaInicio,
                Obra = c.SolicitudEmpleado.Solicitud.ObraId.ToString(),
                Motivo = c.SolicitudEmpleado.Solicitud.Comentarios,

                ClaseValoracion = "Servicio",
                Material = "ALIMENTACION",
                Descripcion = "ALIMENTACION DEDUCIBLE",

                Cantidad = 1,

                UM = "SER",
                Folio = c.SolicitudEmpleado.Solicitud.Folio,

                FechaRealServicio = c.FechaInicio, 
                MontoSinIVA = c.PrecioUnitario,
                IVA = c.PrecioUnitario * 0.16m,
                Monto = c.PrecioUnitario * 1.16m,

                Moneda = "MXN",

                Proveedor = c.Proveedor.Nombre,

                Fecha = c.FechaInicio,
                Alcance = ObtenerAlcanceComida(c.TipoComidaId)
            })
            .ToListAsync(cancellationToken);

            var data = hoteles.Concat(comidas).ToList();

            return _excelService.GenerarReporteServicios(data);
        }

        private static string ObtenerAlcanceComida(int tipo)
        {
            return tipo switch
            {
                (int)TipoComidaEnum.Desayuno => "01 DESAYUNO",
                (int)TipoComidaEnum.Almuerzo => "01 COMIDA",
                (int)TipoComidaEnum.Cena => "01 CENA",
                _ => ""
            };
        }
    }
}
