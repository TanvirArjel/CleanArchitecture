using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.CacheRepositories
{
    public interface IDepartmentCacheRepository : IScopedService
    {
        Task<Department> GetByIdAsync(int departmentId);

        Task<DepartmentDetailsDto> GetDetailsByIdAsync(int departmentId);

        Task<List<DepartmentDetailsDto>> GetListAsync();

        Task<List<DepartmentSelectListDto>> GetSelectListAsync();
    }
}
