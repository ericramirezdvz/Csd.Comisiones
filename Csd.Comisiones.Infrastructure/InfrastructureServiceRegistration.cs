using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Infrastructure.Account;
using Csd.Comisiones.Infrastructure.Email;
using Csd.Comisiones.Infrastructure.Excel;
using Csd.Comisiones.Infrastructure.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(
                configuration.GetSection("SmtpSettings"));

            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            services.AddTransient<IEmailService, SmtpEmailService>();
            services.AddTransient<IFileParserService, FileParserService>();
            services.AddTransient<ISolpedExcelService, SolpedExcelService>();
            services.AddTransient<IActiveDirectoryService, ActiveDirectoryService>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IExcelService, ExcelService>();

            return services;
        }
    }
}
