using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPageClient.ViewModels.DepartmentsViewModels;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace RazorPageClient.Services
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
