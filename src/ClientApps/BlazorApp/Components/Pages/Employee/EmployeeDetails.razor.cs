using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages.Employee
{
    public partial class EmployeeDetails
    {
        private readonly EmployeeService _employeeService;

        public EmployeeDetails(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Parameter]
        public int EmployeeId { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsModel = await _employeeService.GetDetailsByIdAsync(EmployeeId);
        }
    }
}
