using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Employee
{
    public partial class DeleteEmployee
    {
        [Parameter]
        public int EmployeeId { get; set; }

        [Inject]
        private EmployeeService EmployeeService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsModel = await EmployeeService.GetDetailsByIdAsync(EmployeeId);
        }

        private async Task HandleValidSubmit()
        {
            await EmployeeService.DeleteAsync(EmployeeId);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
