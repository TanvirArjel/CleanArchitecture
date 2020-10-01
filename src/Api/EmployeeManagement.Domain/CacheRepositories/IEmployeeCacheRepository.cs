using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.CacheRepositories
{
    public interface IEmployeeCacheRepository : IScopedService
    {
        Task<Employee> GetByIdAsync(long employeeId);

        Task<EmployeeDetailsDto> GetDetailsByIdAsync(long employeeId);
    }
}
