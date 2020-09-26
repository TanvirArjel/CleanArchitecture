using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;

namespace EmployeeManagement.Infrastructure.Data.Repositories
{
    internal interface IRepository<T> : IScopedService
        where T : class
    {
        IQueryable<T> Entities { get; }

        Task<T> GetByIdAsync(object id);

        Task<object[]> InsertAsync(T entity);

        Task InsertAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task UpdateAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);

        Task DeleteAsync(IEnumerable<T> entities);

        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
    }
}
