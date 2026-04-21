using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Infrastructure
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(string username, List<string> roles);
    }
}
