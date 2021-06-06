using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Department
{
    public partial class DeleteDepartment
    {
        [Parameter]
        public int DepartmentId { get; set; }

        [Inject]
        private DepartmentService DepartmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsModel = await DepartmentService.GetByIdAsync(DepartmentId);
        }

        private async Task HandleValidSubmit()
        {
            await DepartmentService.DeleteAsync(DepartmentId);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
