using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using BlazorClientApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlazorClientApp.Services
{
    public interface IDepartmentService : IScopedService
    {
        Task<List<DepartmentDetailsViewModel>> GetDepartmentListAsync();

        Task<SelectList> GetDepartmentSelectListAsync(int? selectedDepartment = null);

        Task CreateDepartmentAsync(CreateDepartmentViewModel createDepartmentViewModel);

        Task<DepartmentDetailsViewModel> GetDepartmentAsync(int departmentId);

        Task UpdateDepartmentAsync(UpdateDepartmentViewModel updateDepartmentViewModel);

        Task DeleteDepartmentAsync(int departmentId);
    }
}
