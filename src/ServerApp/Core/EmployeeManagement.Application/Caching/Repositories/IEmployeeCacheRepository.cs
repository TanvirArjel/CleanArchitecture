using System;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Caching.Repositories
{
    [ScopedService]
    public interface IEmployeeCacheRepository
    {
        Task<Employee> GetByIdAsync(Guid employeeId);

        Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId);

        Task UpdateAsync(Employee employee);
    }
}
