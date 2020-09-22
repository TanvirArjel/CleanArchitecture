using System.Threading.Tasks;
using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorClientApp.Pages.Department
{
    public partial class CreateDepartment
    {
        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        private CreateDepartmentViewModel CreateDepartmentModel { get; set; } = new CreateDepartmentViewModel();

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public async Task HandleValidSubmit()
        {
            await DepartmentService.CreateDepartmentAsync(CreateDepartmentModel);
            NavigationManager.NavigateTo("department/department-list");
        }
    }
}
