using Csd.Comisiones.Application.Contracts.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace Csd.Comisiones.Infrastructure.Account
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        public async Task<bool> ValidateCredentials(string username, string password)
        {
            return await Task.Run(() =>
            {
                using var context = new PrincipalContext(ContextType.Domain, "diavaz.com");
                return context.ValidateCredentials(username, password);
            });
        }
    }
}
