using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.EmployeeViewModels;

namespace BlazorWasmApp.Components.Pages.EmployeeComponents
{
    public partial class EmployeeListComponent
    {
        private readonly EmployeeService _employeeService;

        public EmployeeListComponent(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        private List<EmployeeDetailsViewModel> Employees { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Employees = await _employeeService.GetListAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ErrorMessage = "There is some error.";
            }
        }
    }
}
