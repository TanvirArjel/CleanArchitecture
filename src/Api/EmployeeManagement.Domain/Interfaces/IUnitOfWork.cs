using EmployeeManagement.Common.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Interfaces
{
    public interface IUnitOfWork : IScopedService
    {
        IRepository<T> Repository<T>()
            where T : class;

        int ExecuteSqlCommand(string sql, params object[] parameters);

        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

        void ResetContextState();

        Task SaveChangesAsync();
    }
}
