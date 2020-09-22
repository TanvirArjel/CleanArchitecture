using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorClientApp.Pages.Employee
{
    public partial class EmployeeList
    {
        [Inject]
        private IEmployeeService EmployeeService { get; set; }

        private List<EmployeeDetailsViewModel> Employees { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Employees = await EmployeeService.GetEmployeeListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "There is some error.";
            }
        }
    }
}
