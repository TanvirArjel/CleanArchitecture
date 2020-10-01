using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Repositories
{
    public interface IDepartmentRepository : IScopedService
    {
        IQueryable<Department> Departments { get; }

        Task<List<DepartmentDetailsDto>> GetListAsync();

        Task<List<DepartmentSelectListDto>> GetSelectListAsync();

        Task<Department> GetByIdAsync(int departmentId);

        Task<DepartmentDetailsDto> GetDetailsByIdAsync(int departmentId);

        Task<int> InsertAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
    }
}
