using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.CacheRepositories
{
    public interface IEmployeeCacheRepository : IScopedService
    {
        Task<Employee> GetByIdAsync(long employeeId);
    }
}
