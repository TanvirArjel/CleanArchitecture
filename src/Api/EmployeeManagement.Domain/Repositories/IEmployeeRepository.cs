using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Repositories
{
    public interface IEmployeeRepository : IScopedService
    {
        IQueryable<Employee> Employees { get; }

        Task<Employee> GetByIdAsync(long employeeId);

        Task InsertAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);
    }
}
