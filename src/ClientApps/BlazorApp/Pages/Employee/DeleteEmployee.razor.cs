using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Employee
{
    public partial class DeleteEmployee
    {
        private readonly EmployeeService _employeeService;
        private readonly NavigationManager _navigationManager;

        public DeleteEmployee(EmployeeService employeeService, NavigationManager navigationManager)
        {
            _employeeService = employeeService;
            _navigationManager = navigationManager;
        }

        [Parameter]
        public int EmployeeId { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsModel = await _employeeService.GetDetailsByIdAsync(EmployeeId);
        }

        private async Task HandleValidSubmit()
        {
            await _employeeService.DeleteAsync(EmployeeId);
            _navigationManager.NavigateTo("employee/employee-list");
        }
    }
}
