using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.CacheRepositories
{
    [ScopedService]
    public interface IDepartmentCacheRepository
    {
        Task<List<DepartmentDetailsDto>> GetListAsync();

        Task<List<DepartmentSelectListDto>> GetSelectListAsync();

        Task<Department> GetByIdAsync(int departmentId);

        Task<DepartmentDetailsDto> GetDetailsByIdAsync(int departmentId);
    }
}
