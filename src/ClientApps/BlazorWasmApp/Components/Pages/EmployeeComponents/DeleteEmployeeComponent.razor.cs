using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.EmployeeComponents
{
    public partial class DeleteEmployeeComponent
    {
        private readonly EmployeeService _employeeService;
        private readonly NavigationManager _navigationManager;

        public DeleteEmployeeComponent(EmployeeService employeeService, NavigationManager navigationManager)
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
