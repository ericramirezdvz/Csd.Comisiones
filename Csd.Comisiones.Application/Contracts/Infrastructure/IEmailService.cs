using Csd.Comisiones.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}
