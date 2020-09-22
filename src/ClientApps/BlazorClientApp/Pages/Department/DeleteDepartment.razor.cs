using System.Threading.Tasks;
using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorClientApp.Pages.Department
{
    public partial class DeleteDepartment
    {
        [Parameter]
        public int DepartmentId { get; set; }

        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsModel = await DepartmentService.GetDepartmentAsync(DepartmentId);
        }

        private async Task HandleValidSubmit()
        {
            await DepartmentService.DeleteDepartmentAsync(DepartmentId);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
