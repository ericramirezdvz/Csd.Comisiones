using ClosedXML.Excel;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Features.Reportes.ReporteServicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Infrastructure.Excel
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerarReporteServicios(List<ReporteServiciosDto> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte");

            var headers = new[]
            {
            "AREA", "FECHA", "OBRA", "MOTIVO", "CLASE",
            "MATERIAL", "DESCRIPCION", "CANT", "UM",
            "FOLIO", "FECHA SERVICIO", "MONTO SIN IVA",
            "IVA", "MONTO", "MONEDA", "PROVEEDOR",
            "FECHA", "ALCANCE"
        };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            int row = 2;

            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = item.AreaSolicitante;
                ws.Cell(row, 2).Value = item.FechaSolicitud;
                ws.Cell(row, 3).Value = item.Obra;
                ws.Cell(row, 4).Value = item.Motivo;
                ws.Cell(row, 5).Value = item.ClaseValoracion;
                ws.Cell(row, 6).Value = item.Material;
                ws.Cell(row, 7).Value = item.Descripcion;
                ws.Cell(row, 8).Value = item.Cantidad;
                ws.Cell(row, 9).Value = item.UM;
                ws.Cell(row, 10).Value = item.Folio;
                ws.Cell(row, 11).Value = item.FechaRealServicio;
                ws.Cell(row, 12).Value = item.MontoSinIVA;
                ws.Cell(row, 13).Value = item.IVA;
                ws.Cell(row, 14).Value = item.Monto;
                ws.Cell(row, 15).Value = item.Moneda;
                ws.Cell(row, 16).Value = item.Proveedor;
                ws.Cell(row, 17).Value = item.Fecha;
                ws.Cell(row, 18).Value = item.Alcance;
                row++;
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
