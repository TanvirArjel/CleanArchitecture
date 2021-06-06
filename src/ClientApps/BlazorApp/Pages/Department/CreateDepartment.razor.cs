using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Department
{
    public partial class CreateDepartment
    {
        [Inject]
        private DepartmentService DepartmentService { get; set; }

        private CreateDepartmentViewModel CreateDepartmentModel { get; set; } = new CreateDepartmentViewModel();

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public async Task HandleValidSubmit()
        {
            await DepartmentService.CreateAsync(CreateDepartmentModel);
            NavigationManager.NavigateTo("department/department-list");
        }
    }
}
