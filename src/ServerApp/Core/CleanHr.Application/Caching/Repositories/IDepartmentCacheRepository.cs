using CleanHr.Application.Queries.DepartmentQueries;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Caching.Repositories;

[ScopedService]
public interface IDepartmentCacheRepository
{
    Task<List<DepartmentDto>> GetListAsync();

    Task<Department> GetByIdAsync(Guid departmentId);

    Task<DepartmentDetailsDto> GetDetailsByIdAsync(Guid departmentId);
}
