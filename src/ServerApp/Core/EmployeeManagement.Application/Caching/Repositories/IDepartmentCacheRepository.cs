using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Caching.Repositories
{
    [ScopedService]
    public interface IDepartmentCacheRepository
    {
        Task<List<DepartmentDto>> GetListAsync();

        Task<Department> GetByIdAsync(Guid departmentId);

        Task<DepartmentDetailsDto> GetDetailsByIdAsync(Guid departmentId);

        Task<Guid> InsertAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
    }
}
