using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Csd.Comisiones.Application.Contracts.Infrastructure;

namespace Csd.Comisiones.Infrastructure.Files
{
    public class FileParserService : IFileParserService
    {
        public async Task<List<string>> LeerNumerosEmpleadoAsync(Stream stream, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            var numeros = new List<string>();

            if (extension == ".csv")
            {
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                        numeros.Add(line.Trim());
                }
            }
            else if (extension == ".xlsx" || extension == ".xls")
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                foreach (var row in worksheet.RowsUsed())
                {
                    var value = row.Cell(1).GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                        numeros.Add(value.Trim());
                }
            }
            else
            {
                throw new Exception("Formato no soportado");
            }

            return numeros;
        }
    }
}
