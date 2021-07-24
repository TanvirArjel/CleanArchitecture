using System;
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

        Task<Guid> CreateAsync(CreateDepartmentDto createDepartmentDto);

        Task<SelectList> GetSelectListAsync(Guid? selectedDepartmentId);

        Task<DepartmentDetailsDto> GetByIdAsync(Guid departmentId);

        Task UpdateAsync(UpdateDepartmentDto updateDepartmentDto);

        Task DeleteAsync(Guid departmentId);

        Task<bool> ExistsAsync(Guid departmentId);

        Task<bool> ExistsByNameAsync(string departmentName);

        Task<bool> IsUniqueAsync(Guid departmentId, string departmentName);
    }
}
