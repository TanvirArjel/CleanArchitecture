using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.CacheRepositories
{
    [ScopedService]
    public interface IEmployeeCacheRepository
    {
        Task<Employee> GetByIdAsync(long employeeId);

        Task<EmployeeDetailsDto> GetDetailsByIdAsync(long employeeId);

        Task UpdateAsync(Employee employee);
    }
}
