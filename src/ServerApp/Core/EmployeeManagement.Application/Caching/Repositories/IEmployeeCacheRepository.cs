using EmployeeManagement.Application.Queries.EmployeeQueries;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Caching.Repositories;

[ScopedService]
public interface IEmployeeCacheRepository
{
    Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId);
}
