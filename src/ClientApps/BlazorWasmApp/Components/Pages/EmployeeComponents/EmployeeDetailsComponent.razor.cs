using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.EmployeeComponents
{
    public partial class EmployeeDetailsComponent
    {
        private readonly EmployeeService _employeeService;

        public EmployeeDetailsComponent(EmployeeService employeeService)
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
