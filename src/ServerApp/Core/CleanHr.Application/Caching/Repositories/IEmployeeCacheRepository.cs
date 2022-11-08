using CleanHr.Application.Queries.EmployeeQueries;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Caching.Repositories;

[ScopedService]
public interface IEmployeeCacheRepository
{
    Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId);
}
