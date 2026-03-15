using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
