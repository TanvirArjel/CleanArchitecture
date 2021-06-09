using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Employee
{
    public partial class EmployeeDetails
    {
        private readonly EmployeeService _employeeService;

        [Parameter]
        public int EmployeeId { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsModel = await _employeeService.GetDetailsByIdAsync(EmployeeId);
        }
    }
}
