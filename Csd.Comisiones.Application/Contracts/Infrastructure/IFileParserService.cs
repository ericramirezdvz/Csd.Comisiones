using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Infrastructure
{
    public interface IFileParserService
    {
        Task<List<string>> LeerNumerosEmpleadoAsync(Stream stream, string fileName);
    }
}
