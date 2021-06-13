using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Services
{
    [ScopedService]
    public interface IDepartmentService
    {
        Task<List<DepartmentDetailsDto>> GetListAsync();

        Task<int> CreateAsync(CreateDepartmentDto createDepartmentDto);

        Task<SelectList> GetSelectListAsync(int? selectedDepartmentId);

        Task<DepartmentDetailsDto> GetByIdAsync(int departmentId);

        Task UpdateAsync(UpdateDepartmentDto updateDepartmentDto);

        Task DeleteAsync(int departmentId);

        Task<bool> ExistsAsync(int departmentId);

        Task<bool> ExistsByNameAsync(string departmentName);

        Task<bool> IsUniqueAsync(int departmentId, string departmentName);
    }
}
