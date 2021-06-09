using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;

namespace BlazorApp.Pages.Employee
{
    public partial class EmployeeList
    {
        private readonly EmployeeService _employeeService;

        public EmployeeList(EmployeeService employeeService)
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
            catch (Exception)
            {
                ErrorMessage = "There is some error.";
            }
        }
    }
}
