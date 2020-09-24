using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Application.Services
{
    public interface IDepartmentService : IScopedService
    {
        Task<List<DepartmentDetailsDto>> GetDepartmentListAsync();

        Task<int> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto);

        Task<SelectList> GetDepartmentSelectListAsync(int? selectedDepartmentId);

        Task<DepartmentDetailsDto> GetDepartmentAsync(int departmentId);

        Task UpdateDepartmentAsync(UpdateDepartmentDto updateDepartmentDto);

        Task DeleteDepartment(int employeeId);

        Task<bool> DepartmentExistsAsync(int departmentId);
    }
}
