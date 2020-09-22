using System.Threading.Tasks;
using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorClientApp.Pages.Employee
{
    public partial class DeleteEmployee
    {
        [Parameter]
        public int EmployeeId { get; set; }

        [Inject]
        private IEmployeeService EmployeeService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsModel = await EmployeeService.GetEmployeeDetailsAsync(EmployeeId);
        }

        private async Task HandleValidSubmit()
        {
            await EmployeeService.DeleteEmployee(EmployeeId);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
