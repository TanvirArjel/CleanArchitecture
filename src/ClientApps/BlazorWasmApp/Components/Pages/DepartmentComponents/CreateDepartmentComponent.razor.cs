using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.DepartmentComponents
{
    public partial class CreateDepartmentComponent
    {
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public CreateDepartmentComponent(DepartmentService departmentService, NavigationManager navigationManager)
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
