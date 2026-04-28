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

        // ═══════════════════════════════════════════════════════════
        //  GENERADOR DE SERVICIOS Y SUBCONTRATOS — por proveedor
        // ═══════════════════════════════════════════════════════════

        public Dictionary<int, (string NombreProveedor, string Correo, byte[] Excel)> GenerarExcelPorProveedor(Solicitud solicitud)
        {
            var result = new Dictionary<int, (string, string, byte[])>();

            var empleados = solicitud.Empleados
                .Where(e => e.TipoAsignacion != TipoAsignacionEnum.Pago)
                .ToList();

            // Collect all active services with their provider
            var servicios = new List<(int ProveedorId, string ProveedorNombre, string ProveedorCorreo, ServicioGenerador Servicio)>();

            foreach (var emp in empleados)
            {
                foreach (var h in emp.Hoteles.Where(h =>
                    h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada &&
                    h.EstatusDetalleId != (int)EstatusDetalleEnum.Rechazada &&
                    h.ProveedorId.HasValue))
                {
                    var tipoHab = h.TipoHabitacionId == 2 ? "HABITACIÓN DOBLE" : "HABITACIÓN SENCILLA";
                    servicios.Add((
                        h.ProveedorId!.Value,
                        h.Proveedor?.Nombre ?? "",
                        h.Proveedor?.Correo ?? "",
                        new ServicioGenerador
                        {
                            Concepto = "HOSPEDAJE",
                            TipoDetalle = tipoHab,
                            EmpleadoNombre = emp.EsExterno ? (emp.NombreExterno ?? "Externo") : (emp.Empleado?.NombreCompleto ?? "Sin nombre"),
                            NumeroPersonal = emp.EsExterno ? "" : (emp.Empleado?.NumeroEmpleado ?? ""),
                            FechaInicio = h.FechaInicio.Date,
                            FechaFin = h.FechaFin.Date,
                            PrecioUnitario = h.PrecioUnitario,
                            EsHotel = true,
                            TipoHabitacionId = h.TipoHabitacionId
                        }));
                }

                foreach (var c in emp.Comidas.Where(c =>
                    c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada &&
                    c.EstatusDetalleId != (int)EstatusDetalleEnum.Rechazada &&
                    c.ProveedorId.HasValue))
                {
                    var tipoComida = c.TipoComidaId switch
                    {
                        1 => "DESAYUNO",
                        2 => "COMIDA",
                        3 => "CENA",
                        _ => $"ALIMENTO #{c.TipoComidaId}"
                    };

                    servicios.Add((
                        c.ProveedorId!.Value,
                        c.Proveedor?.Nombre ?? "",
                        c.Proveedor?.Correo ?? "",
                        new ServicioGenerador
                        {
                            Concepto = tipoComida,
                            TipoDetalle = tipoComida,
                            EmpleadoNombre = emp.EsExterno ? (emp.NombreExterno ?? "Externo") : (emp.Empleado?.NombreCompleto ?? "Sin nombre"),
                            NumeroPersonal = emp.EsExterno ? "" : (emp.Empleado?.NumeroEmpleado ?? ""),
                            FechaInicio = c.FechaInicio.Date,
                            FechaFin = c.FechaFin.Date,
                            PrecioUnitario = c.PrecioUnitario,
                            EsHotel = false,
                            TipoHabitacionId = 0
                        }));
                }
            }

            var porProveedor = servicios.GroupBy(s => s.ProveedorId);

            foreach (var grupo in porProveedor)
            {
                var provNombre = grupo.First().ProveedorNombre;
                var provCorreo = grupo.First().ProveedorCorreo;
                var excel = BuildGeneradorExcel(solicitud, provNombre, grupo.Select(g => g.Servicio).ToList());
                result[grupo.Key] = (provNombre, provCorreo, excel);
            }

            return result;
        }

        private static byte[] BuildGeneradorExcel(
            Solicitud solicitud,
            string proveedorNombre,
            List<ServicioGenerador> servicios)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("GENERADOR");

            // Determine the month from FechaInicio for the day columns
            var mesReferencia = solicitud.FechaInicio.Date;
            var diasEnMes = DateTime.DaysInMonth(mesReferencia.Year, mesReferencia.Month);
            var mesNombre = mesReferencia.ToString("MMMM", new System.Globalization.CultureInfo("es-MX")).ToUpper();

            // ── HEADER SECTION ──
            // Row 3: Company name
            ws.Cell("C3").Value = "CONSTRUCTORA SUBACUÁTICA DIAVAZ, S.A. DE C.V.";
            ws.Cell("C3").Style.Font.Bold = true;
            ws.Cell("C3").Style.Font.FontSize = 14;
            ws.Range("C3:AO3").Merge();
            ws.Cell("C3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Row 4: Department
            ws.Cell("C4").Value = "GERENCIA DE ABASTECIMIENTOS";
            ws.Cell("C4").Style.Font.Bold = true;
            ws.Range("C4:AO4").Merge();
            ws.Cell("C4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Row 5: Document title
            ws.Cell("C5").Value = "GENERADOR DE SERVICIOS Y SUBCONTRATOS";
            ws.Cell("C5").Style.Font.Bold = true;
            ws.Cell("C5").Style.Font.FontSize = 11;
            ws.Range("C5:AO5").Merge();
            ws.Cell("C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C5").Style.Font.Underline = XLFontUnderlineValues.Single;

            // Row 7: FOLIO
            ws.Cell("AO7").Value = "FOLIO:";
            ws.Cell("AO7").Style.Font.Bold = true;
            ws.Cell("AP7").Value = solicitud.Folio;

            // Row 11: PROVEEDOR
            ws.Cell("B11").Value = "PROVEEDOR:";
            ws.Cell("B11").Style.Font.Bold = true;
            ws.Cell("C11").Value = proveedorNombre;

            // SERVICIOS checkbox (marked)
            ws.Cell("AD15").Value = "SERVICIOS";
            ws.Cell("AD15").Style.Font.Bold = true;
            ws.Cell("AE15").Value = "X";
            ws.Cell("AE15").Style.Font.Bold = true;

            // Row 13: PROYECTO/OBRA
            ws.Cell("B13").Value = "PROYECTO/OBRA:";
            ws.Cell("B13").Style.Font.Bold = true;
            ws.Cell("C13").Value = solicitud.Obra?.Nombre ?? "";

            // Row 17: EMBARCACIÓN/SITIO
            ws.Cell("B17").Value = "EMBARCACIÓN/SITIO:";
            ws.Cell("B17").Style.Font.Bold = true;
            ws.Cell("C17").Value = solicitud.Ciudad?.Nombre ?? "";

            // ── TABLE HEADER — Row 19-20 ──
            int headerRow1 = 19;
            int headerRow2 = 20;

            // Fixed columns: B=POSICIÓN, C=CONCEPTO, D=UNIDAD, E=SERIE/PERSONAL, F=MARCA,
            // G=FECHA INICIO RENTA/SERV, H=PRECIO UNITARIO
            var fixedHeaders = new[] { "POSICIÓN", "CONCEPTO", "UNIDAD", "SERIE /\nPERSONAL", "MARCA", "FECHA\nINICIO\nRENTA/SERV", "PRECIO\nUNITARIO" };
            int colStart = 2; // Column B
            for (int i = 0; i < fixedHeaders.Length; i++)
            {
                var cell = ws.Cell(headerRow1, colStart + i);
                cell.Value = fixedHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(headerRow1, colStart + i, headerRow2, colStart + i).Merge();
            }

            // MES header + day columns start at column I (col 9)
            int dayColStart = 9; // Column I
            ws.Cell(headerRow1, dayColStart).Value = $"MES:";
            ws.Cell(headerRow1, dayColStart).Style.Font.Bold = true;
            ws.Cell(headerRow1, dayColStart + 2).Value = mesNombre;
            ws.Cell(headerRow1, dayColStart + 2).Style.Font.Bold = true;

            // Day number sub-headers in row 20
            for (int d = 1; d <= diasEnMes; d++)
            {
                var cell = ws.Cell(headerRow2, dayColStart + d - 1);
                cell.Value = d;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 8;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Column(dayColStart + d - 1).Width = 3.5;
            }

            // After day columns: TOTAL DIAS/HH, COSTO TOTAL MXP, COSTO TOTAL USD
            int colTotalDias = dayColStart + diasEnMes;
            int colCostoMxp = colTotalDias + 1;
            int colCostoUsd = colCostoMxp + 1;

            ws.Cell(headerRow1, colTotalDias).Value = "TOTAL\nDIAS /\nHH";
            ws.Cell(headerRow1, colTotalDias).Style.Font.Bold = true;
            ws.Cell(headerRow1, colTotalDias).Style.Alignment.WrapText = true;
            ws.Cell(headerRow1, colTotalDias).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(headerRow1, colTotalDias, headerRow2, colTotalDias).Merge();
            ws.Cell(headerRow1, colTotalDias).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Cell(headerRow1, colCostoMxp).Value = "COSTO TOTAL";
            ws.Cell(headerRow1, colCostoMxp).Style.Font.Bold = true;
            ws.Cell(headerRow1, colCostoMxp).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(headerRow2, colCostoMxp).Value = "MXP";
            ws.Cell(headerRow2, colCostoMxp).Style.Font.Bold = true;
            ws.Cell(headerRow2, colCostoMxp).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(headerRow1, colCostoMxp).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(headerRow2, colCostoMxp).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Cell(headerRow1, colCostoUsd).Value = "COSTO TOTAL";
            ws.Cell(headerRow1, colCostoUsd).Style.Font.Bold = true;
            ws.Cell(headerRow1, colCostoUsd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(headerRow2, colCostoUsd).Value = "USD";
            ws.Cell(headerRow2, colCostoUsd).Style.Font.Bold = true;
            ws.Cell(headerRow2, colCostoUsd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(headerRow1, colCostoUsd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(headerRow2, colCostoUsd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // ── DATA ROWS ──
            // Group services into positions by Concepto + TipoHabitacion (for hotels)
            var posiciones = servicios
                .GroupBy(s => s.EsHotel
                    ? $"{s.Concepto}:{s.TipoHabitacionId}"
                    : s.Concepto)
                .OrderBy(g => g.First().EsHotel ? 0 : 1) // Hotels first
                .ThenBy(g => g.Key)
                .ToList();

            int currentRow = headerRow2 + 1; // Row 21
            int posNum = 0;
            decimal totalGeneralMxp = 0;

            foreach (var pos in posiciones)
            {
                posNum += 10;
                var first = pos.First();
                var esHotel = first.EsHotel;

                // For hotels: calculate nights (FechaFin - FechaInicio in days)
                // For meals: calculate calendar days inclusive
                int totalDias;
                decimal precioUnitario = first.PrecioUnitario;
                int cantidad;
                string concepto;

                if (esHotel)
                {
                    concepto = "HOSPEDAJE";
                    var empleadosList = pos.Select(s => s.EmpleadoNombre).ToList();
                    var noches = pos.Select(s => Math.Max((s.FechaFin - s.FechaInicio).Days, 1)).First();

                    if (first.TipoHabitacionId == 2)
                    {
                        // Double rooms: ceil(employees/2)
                        cantidad = (int)Math.Ceiling(empleadosList.Count / 2.0);
                    }
                    else
                    {
                        cantidad = empleadosList.Count;
                    }
                    totalDias = noches;
                }
                else
                {
                    concepto = first.Concepto;
                    var diasPorEmp = pos.Select(s => (s.FechaFin - s.FechaInicio).Days + 1).First();
                    cantidad = pos.Count(); // one per employee
                    totalDias = diasPorEmp;
                }

                decimal costoTotalMxp = cantidad * totalDias * precioUnitario;
                totalGeneralMxp += costoTotalMxp;

                // Data row
                ws.Cell(currentRow, 2).Value = posNum; // POSICIÓN
                ws.Cell(currentRow, 3).Value = concepto; // CONCEPTO
                ws.Cell(currentRow, 4).Value = cantidad; // UNIDAD (quantity)
                ws.Cell(currentRow, 7).Value = pos.Min(s => s.FechaInicio); // FECHA INICIO
                ws.Cell(currentRow, 7).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(currentRow, 8).Value = precioUnitario; // PRECIO UNITARIO
                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";

                // Mark days with X in the day grid
                foreach (var servicio in pos)
                {
                    for (var d = servicio.FechaInicio; d < (esHotel ? servicio.FechaFin : servicio.FechaFin.AddDays(1)); d = d.AddDays(1))
                    {
                        if (d.Month == mesReferencia.Month && d.Year == mesReferencia.Year)
                        {
                            var dayCol = dayColStart + d.Day - 1;
                            ws.Cell(currentRow, dayCol).Value = "X";
                            ws.Cell(currentRow, dayCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(currentRow, dayCol).Style.Font.FontSize = 8;
                        }
                    }
                }

                // TOTAL DIAS
                ws.Cell(currentRow, colTotalDias).Value = totalDias;
                ws.Cell(currentRow, colTotalDias).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // COSTO TOTAL MXP
                ws.Cell(currentRow, colCostoMxp).Value = costoTotalMxp;
                ws.Cell(currentRow, colCostoMxp).Style.NumberFormat.Format = "$#,##0.00";

                // COSTO TOTAL USD — leave empty (MXN only)
                ws.Cell(currentRow, colCostoUsd).Value = "-";
                ws.Cell(currentRow, colCostoUsd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Apply borders to data row
                for (int c = 2; c <= colCostoUsd; c++)
                    ws.Cell(currentRow, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                currentRow++;

                // TEXTO DE POSICIÓN row
                ws.Cell(currentRow, 2).Value = "TEXTO DE POSICIÓN :";
                ws.Cell(currentRow, 2).Style.Font.Bold = true;
                ws.Cell(currentRow, 2).Style.Font.FontSize = 8;

                var textoPos = esHotel
                    ? $"{first.TipoDetalle} - {string.Join(" - ", pos.Select(s => s.EmpleadoNombre))}"
                    : $"{first.TipoDetalle} - {string.Join(" - ", pos.Select(s => s.EmpleadoNombre))}";
                ws.Cell(currentRow, 3).Value = textoPos;
                ws.Cell(currentRow, 3).Style.Font.FontSize = 8;
                ws.Range(currentRow, 3, currentRow, dayColStart + diasEnMes - 1).Merge();

                // Borders for text row
                for (int c = 2; c <= colCostoUsd; c++)
                    ws.Cell(currentRow, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Row(currentRow).Style.Fill.BackgroundColor = XLColor.FromHtml("#d9e2f3");

                currentRow++;
            }

            // ── FOOTER: IVA and RETENCIÓN ──
            currentRow += 2;
            var ivaMonto = Math.Round(totalGeneralMxp * TasaIVA, 2);

            ws.Cell(currentRow, colTotalDias).Value = "IVA 16%:";
            ws.Cell(currentRow, colTotalDias).Style.Font.Bold = true;
            ws.Cell(currentRow, colTotalDias).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(currentRow, colCostoMxp).Value = ivaMonto;
            ws.Cell(currentRow, colCostoMxp).Style.NumberFormat.Format = "$#,##0.00";
            ws.Cell(currentRow, colCostoUsd).Value = "-";
            ws.Cell(currentRow, colCostoUsd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
            ws.Cell(currentRow, colTotalDias).Value = "RETENCIÓN:";
            ws.Cell(currentRow, colTotalDias).Style.Font.Bold = true;
            ws.Cell(currentRow, colTotalDias).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(currentRow, colCostoMxp).Value = "-";
            ws.Cell(currentRow, colCostoMxp).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(currentRow, colCostoUsd).Value = "-";
            ws.Cell(currentRow, colCostoUsd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // NOTAS
            currentRow++;
            ws.Cell(currentRow, 2).Value = "NOTAS:";
            ws.Cell(currentRow, 2).Style.Font.Bold = true;

            // Set column widths for fixed columns
            ws.Column(2).Width = 12; // POSICIÓN
            ws.Column(3).Width = 22; // CONCEPTO
            ws.Column(4).Width = 8;  // UNIDAD
            ws.Column(5).Width = 12; // SERIE/PERSONAL
            ws.Column(6).Width = 8;  // MARCA
            ws.Column(7).Width = 12; // FECHA INICIO
            ws.Column(8).Width = 12; // PRECIO UNITARIO
            ws.Column(colTotalDias).Width = 10;
            ws.Column(colCostoMxp).Width = 14;
            ws.Column(colCostoUsd).Width = 14;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private class ServicioGenerador
        {
            public string Concepto { get; set; } = "";
            public string TipoDetalle { get; set; } = "";
            public string EmpleadoNombre { get; set; } = "";
            public string NumeroPersonal { get; set; } = "";
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public decimal PrecioUnitario { get; set; }
            public bool EsHotel { get; set; }
            public int TipoHabitacionId { get; set; }
        }

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
            // Collect all active hotel records across employees
            var hotelRecords = solicitud.Empleados
                .Where(e => e.TipoAsignacion != TipoAsignacionEnum.Pago)
                .SelectMany(emp => emp.Hoteles
                    .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada
                             && h.ProveedorId.HasValue)
                    .Select(h => new { Empleado = emp, Hotel = h }))
                .ToList();

            // Group by (Proveedor, TipoHabitacion, FechaInicio, FechaFin)
            // so double rooms shared by 2 employees consolidate into one partida
            var grupos = hotelRecords
                .GroupBy(r => new
                {
                    r.Hotel.ProveedorId,
                    r.Hotel.TipoHabitacionId,
                    FechaInicio = r.Hotel.FechaInicio.Date,
                    FechaFin = r.Hotel.FechaFin.Date
                })
                .OrderBy(g => g.Key.FechaInicio)
                .ToList();

            int row = 2;
            var obraNombre = solicitud.Obra?.Nombre ?? "";
            var motivoNombre = solicitud.MotivoSolicitud?.Nombre ?? "";
            var folio = solicitud.Folio;
            var fechaSolicitud = solicitud.FechaCreacion;

            foreach (var grupo in grupos)
            {
                var numEmpleados = grupo.Count();
                var esDoble = grupo.Key.TipoHabitacionId == 2;
                var tipoHab = esDoble ? "HAB DOBLE" : "HAB SENCILLA";

                // Double rooms: 2 employees per room
                var habitaciones = esDoble
                    ? (int)Math.Ceiling(numEmpleados / 2.0)
                    : numEmpleados;

                var costoUnitarioPorNoche = grupo.First().Hotel.PrecioUnitario;
                var proveedorNombre = grupo.First().Hotel.Proveedor?.Nombre ?? "";

                // Calculate nights
                var noches = (grupo.Key.FechaFin - grupo.Key.FechaInicio).Days;
                if (noches < 1) noches = 1;

                var totalHabNoches = habitaciones * noches;
                var costoTotal = costoUnitarioPorNoche * totalHabNoches;
                var ish = Math.Round(costoTotal * TasaISH, 2);
                var iva = Math.Round(costoTotal * TasaIVA, 2);
                var monto = costoTotal + ish + iva;

                // ALCANCE: e.g., "01 HAB SENCILLA", "10 HAB DOBLES"
                var alcance = $"{totalHabNoches:D2} {tipoHab}{(totalHabNoches > 1 && esDoble ? "S" : "")}";

                // FECHA string
                var fechaIni = grupo.Key.FechaInicio;
                var fechaFinReal = grupo.Key.FechaFin.AddDays(-1); // last night
                var fechaStr = noches == 1
                    ? fechaIni.ToString("d/M/yyyy")
                    : $"{fechaIni:dd} AL {fechaFinReal:dd} DE {fechaFinReal:MMM}".ToUpper();

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
