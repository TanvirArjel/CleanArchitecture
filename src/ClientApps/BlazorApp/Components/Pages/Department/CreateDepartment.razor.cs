using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages.Department
{
    public partial class CreateDepartment
    {
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public CreateDepartment(DepartmentService departmentService, NavigationManager navigationManager)
        {
            _departmentService = departmentService;
            _navigationManager = navigationManager;
        }

        private CreateDepartmentViewModel CreateDepartmentModel { get; set; } = new CreateDepartmentViewModel();

        public async Task HandleValidSubmit()
        {
            await _departmentService.CreateAsync(CreateDepartmentModel);
            _navigationManager.NavigateTo("department/department-list");
        }
    }
}
