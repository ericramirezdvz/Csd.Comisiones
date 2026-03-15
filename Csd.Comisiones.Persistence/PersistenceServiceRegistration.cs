using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ComisionesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ComisionesConnectionString")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ComisionesDbContext>());
            
            services.AddScoped<IProveedorRepository, ProveedorRepository>();
            services.AddScoped<ISolicitudRepository, SolicitudRepository>();

            return services;
        }
    }
}
