using ClosedXML.Excel;
using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Infrastructure.Files
{
    public class FileParserService : IFileParserService
    {
        public List<EmpleadoImportadoDto> LeerNumerosEmpleadoAsync(Stream stream, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            var lista = new List<EmpleadoImportadoDto>();

            if (extension == ".xlsx" || extension == ".xls")
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                foreach (var row in worksheet.RowsUsed().Skip(1)) // saltar encabezado
                {
                    var ficha = row.Cell(2).GetString()?.Trim();   // COLUMNA B
                    var externo = row.Cell(3).GetString()?.Trim(); // COLUMNA C

                    if (!string.IsNullOrWhiteSpace(ficha) || !string.IsNullOrWhiteSpace(externo))
                    {
                        lista.Add(new EmpleadoImportadoDto
                        {
                            NumeroEmpleado = string.IsNullOrWhiteSpace(ficha) ? null : ficha,
                            NombreExterno = string.IsNullOrWhiteSpace(externo) ? null : externo
                        });
                    }
                }
            }
            else
            {
                throw new Exception("Formato no soportado");
            }

            return lista;
        }
    }
}
