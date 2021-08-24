using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.CacheRepositories
{
    [ScopedService]
    public interface IDepartmentCacheRepository
    {
        Task<List<DepartmentDetailsDto>> GetListAsync();

        Task<List<DepartmentSelectListDto>> GetSelectListAsync();

        Task<Department> GetByIdAsync(Guid departmentId);

        Task<DepartmentDetailsDto> GetDetailsByIdAsync(Guid departmentId);

        Task<Guid> InsertAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
    }
}
