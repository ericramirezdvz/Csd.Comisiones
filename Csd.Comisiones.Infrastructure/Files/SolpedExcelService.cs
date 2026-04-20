using ClosedXML.Excel;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;

namespace Csd.Comisiones.Infrastructure.Files
{
    public class SolpedExcelService : ISolpedExcelService
    {
        private const decimal TasaIVA = 0.16m;
        private const decimal TasaISH = 0.03m;

        public byte[] GenerarExcelSolped(Solicitud solicitud)
        {
            using var workbook = new XLWorkbook();

            BuildHojaAlimentacion(workbook, solicitud);
            BuildHojaHospedaje(workbook, solicitud);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ═══════════════════════════════════════════════════════════
        //  HOJA 1: Alimentación
        // ═══════════════════════════════════════════════════════════
        private static void BuildHojaAlimentacion(XLWorkbook workbook, Solicitud solicitud)
        {
            var ws = workbook.Worksheets.Add("Alimentación");

            // ── Headers ──
            var headers = new[]
            {
                "AREA SOLICITANTE", "FECHA DE SOLICITUD STAFF", "OBRA", "MOTIVO",
                "CLASE DE VALORACIÓN", "MATERIAL", "DESCRIPCIÓN", "CANT.", "UM",
                "FOLIO", "FECHA REAL DEL SERVICIO", "MONTO SIN IVA", "IVA", "MONTO",
                "MONEDA", "PROVEEDOR", "FECHA", "ALCANCE", "FOLIO"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#06205c");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // ── Data rows ──
            // Group food services by employee+provider+contiguous date range
            var empleados = solicitud.Empleados
                .Where(e => e.TipoAsignacion != TipoAsignacionEnum.Pago)
                .ToList();

            int row = 2;
            var areaNombre = solicitud.Area?.Nombre ?? "";
            var obraNombre = solicitud.Obra?.Nombre ?? "";
            var motivoNombre = solicitud.MotivoSolicitud?.Nombre ?? "";
            var folio = solicitud.Folio;
            var fechaSolicitud = solicitud.FechaCreacion;

            foreach (var emp in empleados)
            {
                var comidasActivas = emp.Comidas
                    .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada
                             && c.ProveedorId.HasValue)
                    .ToList();

                if (!comidasActivas.Any()) continue;

                // Group by provider, then build rows per contiguous date range
                var porProveedor = comidasActivas
                    .GroupBy(c => c.ProveedorId!.Value)
                    .ToList();

                foreach (var grupoProveedor in porProveedor)
                {
                    var proveedorNombre = grupoProveedor.First().Proveedor?.Nombre ?? "";

                    // Find the overall date range for this employee+provider
                    var fechaMin = grupoProveedor.Min(c => c.FechaInicio).Date;
                    var fechaMax = grupoProveedor.Max(c => c.FechaFin).Date;

                    // Build day-by-day breakdown
                    var dias = new List<DateTime>();
                    for (var d = fechaMin; d <= fechaMax; d = d.AddDays(1))
                        dias.Add(d);

                    // Group contiguous days where at least one meal exists
                    var diasConServicio = dias.Where(dia =>
                        grupoProveedor.Any(c => dia >= c.FechaInicio.Date && dia <= c.FechaFin.Date))
                        .OrderBy(d => d)
                        .ToList();

                    if (!diasConServicio.Any()) continue;

                    // Build contiguous ranges
                    var ranges = GroupContiguousDates(diasConServicio);

                    foreach (var range in ranges)
                    {
                        // Count meals per type in this range
                        int totalDesayunos = 0, totalComidas = 0, totalCenas = 0;

                        for (var d = range.start; d <= range.end; d = d.AddDays(1))
                        {
                            if (grupoProveedor.Any(c => c.TipoComidaId == 1
                                && d >= c.FechaInicio.Date && d <= c.FechaFin.Date))
                                totalDesayunos++;
                            if (grupoProveedor.Any(c => c.TipoComidaId == 2
                                && d >= c.FechaInicio.Date && d <= c.FechaFin.Date))
                                totalComidas++;
                            if (grupoProveedor.Any(c => c.TipoComidaId == 3
                                && d >= c.FechaInicio.Date && d <= c.FechaFin.Date))
                                totalCenas++;
                        }

                        int cantidadTotal = totalDesayunos + totalComidas + totalCenas;
                        if (cantidadTotal == 0) continue;

                        // Calculate cost: sum unit prices × days for each meal type
                        decimal montoSinIva = 0;
                        foreach (var comida in grupoProveedor)
                        {
                            var diasEnRango = 0;
                            for (var d = range.start; d <= range.end; d = d.AddDays(1))
                            {
                                if (d >= comida.FechaInicio.Date && d <= comida.FechaFin.Date)
                                    diasEnRango++;
                            }
                            montoSinIva += comida.PrecioUnitario * diasEnRango;
                        }

                        decimal iva = Math.Round(montoSinIva * TasaIVA, 2);
                        decimal monto = montoSinIva + iva;

                        // Build ALCANCE string
                        var alcanceParts = new List<string>();
                        if (totalDesayunos > 0)
                            alcanceParts.Add($"{totalDesayunos:D2} DESAYUNO{(totalDesayunos > 1 ? "S" : "")}");
                        if (totalComidas > 0)
                            alcanceParts.Add($"{totalComidas:D2} COMIDA{(totalComidas > 1 ? "S" : "")}");
                        if (totalCenas > 0)
                            alcanceParts.Add($"{totalCenas:D2} CENA{(totalCenas > 1 ? "S" : "")}");
                        var alcance = string.Join(", ", alcanceParts);

                        // Build FECHA string
                        var fechaStr = range.start == range.end
                            ? range.start.ToString("d/M/yyyy")
                            : $"{range.start:dd} AL {range.end:dd} DE {range.end:MMM}".ToUpper();

                        // Write row
                        ws.Cell(row, 1).Value = areaNombre;
                        ws.Cell(row, 2).Value = fechaSolicitud;
                        ws.Cell(row, 2).Style.DateFormat.Format = "dd/MM/yyyy";
                        ws.Cell(row, 3).Value = obraNombre;
                        ws.Cell(row, 4).Value = motivoNombre;
                        ws.Cell(row, 5).Value = "Servicio";
                        ws.Cell(row, 6).Value = "136491";
                        ws.Cell(row, 7).Value = "ALIMENTACION DEDUCIBLE";
                        ws.Cell(row, 8).Value = cantidadTotal;
                        ws.Cell(row, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 9).Value = "SER";
                        ws.Cell(row, 10).Value = folio;
                        ws.Cell(row, 11).Value = range.start;
                        ws.Cell(row, 11).Style.DateFormat.Format = "dd-MMM-yy";
                        ws.Cell(row, 12).Value = montoSinIva;
                        ws.Cell(row, 12).Style.NumberFormat.Format = "$#,##0.00";
                        ws.Cell(row, 13).Value = iva;
                        ws.Cell(row, 13).Style.NumberFormat.Format = "$#,##0.00";
                        ws.Cell(row, 14).Value = monto;
                        ws.Cell(row, 14).Style.NumberFormat.Format = "$#,##0.00";
                        ws.Cell(row, 15).Value = "MXN";
                        ws.Cell(row, 16).Value = proveedorNombre;
                        ws.Cell(row, 17).Value = fechaStr;
                        ws.Cell(row, 18).Value = alcance;
                        ws.Cell(row, 19).Value = folio;

                        row++;
                    }
                }
            }

            // Auto-fit columns
            ws.Columns().AdjustToContents();
        }

        // ═══════════════════════════════════════════════════════════
        //  HOJA 2: HOSPEDAJE
        // ═══════════════════════════════════════════════════════════
        private static void BuildHojaHospedaje(XLWorkbook workbook, Solicitud solicitud)
        {
            var ws = workbook.Worksheets.Add("HOSPEDAJE");

            // ── Headers ──
            var headers = new[]
            {
                "FECHA DE SOLICITUD STAFF", "OBRA", "MOTIVO",
                "CLASE DE VALORACIÓN", "DESCRIPCIÓN", "CANT.", "UM",
                "COSTO UNITARIO", "ISH", "IVA", "MONTO",
                "MONEDA", "PROVEEDOR", "FECHA", "ALCANCE", "FOLIO"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#06205c");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // ── Data rows ──
            var empleados = solicitud.Empleados
                .Where(e => e.TipoAsignacion != TipoAsignacionEnum.Pago)
                .ToList();

            int row = 2;
            var obraNombre = solicitud.Obra?.Nombre ?? "";
            var motivoNombre = solicitud.MotivoSolicitud?.Nombre ?? "";
            var folio = solicitud.Folio;
            var fechaSolicitud = solicitud.FechaCreacion;

            foreach (var emp in empleados)
            {
                var hotelesActivos = emp.Hoteles
                    .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada
                             && h.ProveedorId.HasValue)
                    .OrderBy(h => h.FechaInicio)
                    .ToList();

                foreach (var hotel in hotelesActivos)
                {
                    var proveedorNombre = hotel.Proveedor?.Nombre ?? "";
                    var tipoHab = hotel.TipoHabitacionId == 2 ? "HAB DOBLE" : "HAB SENCILLA";
                    var costoUnitario = hotel.PrecioUnitario;

                    // Calculate nights
                    var noches = (hotel.FechaFin.Date - hotel.FechaInicio.Date).Days;
                    if (noches < 1) noches = 1;

                    var costoTotal = costoUnitario * noches;
                    var ish = Math.Round(costoTotal * TasaISH, 2);
                    var iva = Math.Round(costoTotal * TasaIVA, 2);
                    var monto = costoTotal + ish + iva;

                    // ALCANCE: e.g., "01 HAB SENCILLA", "05 HAB DOBLES"
                    var alcance = $"{noches:D2} {tipoHab}{(noches > 1 && tipoHab == "HAB DOBLE" ? "S" : "")}";

                    // FECHA string
                    var fechaStr = noches == 1
                        ? hotel.FechaInicio.ToString("d/M/yyyy")
                        : $"{hotel.FechaInicio:dd} AL {hotel.FechaFin.AddDays(-1):dd} DE {hotel.FechaFin.AddDays(-1):MMM}".ToUpper();

                    ws.Cell(row, 1).Value = fechaSolicitud;
                    ws.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Cell(row, 2).Value = obraNombre;
                    ws.Cell(row, 3).Value = motivoNombre;
                    ws.Cell(row, 4).Value = "Servicio";
                    ws.Cell(row, 5).Value = "HOSPEDAJE";
                    ws.Cell(row, 6).Value = 1.00;
                    ws.Cell(row, 6).Style.NumberFormat.Format = "0.00";
                    ws.Cell(row, 7).Value = "SER";
                    ws.Cell(row, 8).Value = costoTotal;
                    ws.Cell(row, 8).Style.NumberFormat.Format = "$#,##0.00";
                    ws.Cell(row, 9).Value = ish;
                    ws.Cell(row, 9).Style.NumberFormat.Format = "$#,##0.00";
                    ws.Cell(row, 10).Value = iva;
                    ws.Cell(row, 10).Style.NumberFormat.Format = "$#,##0.00";
                    ws.Cell(row, 11).Value = monto;
                    ws.Cell(row, 11).Style.NumberFormat.Format = "$#,##0.00";
                    ws.Cell(row, 12).Value = "MXN";
                    ws.Cell(row, 13).Value = proveedorNombre;
                    ws.Cell(row, 14).Value = fechaStr;
                    ws.Cell(row, 15).Value = alcance;
                    ws.Cell(row, 16).Value = folio;

                    row++;
                }
            }

            // Auto-fit columns
            ws.Columns().AdjustToContents();
        }

        // ── Helpers ──
        private static List<(DateTime start, DateTime end)> GroupContiguousDates(List<DateTime> dates)
        {
            if (dates.Count == 0) return new();

            var result = new List<(DateTime, DateTime)>();
            var start = dates[0];
            var prev = dates[0];

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - prev).Days > 1)
                {
                    result.Add((start, prev));
                    start = dates[i];
                }
                prev = dates[i];
            }
            result.Add((start, prev));

            return result;
        }
    }
}
