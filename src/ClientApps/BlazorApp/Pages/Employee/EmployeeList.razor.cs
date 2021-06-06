using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Employee
{
    public partial class EmployeeList
    {
        [Inject]
        private EmployeeService EmployeeService { get; set; }

        private List<EmployeeDetailsViewModel> Employees { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Employees = await EmployeeService.GetListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "There is some error.";
            }
        }
    }
}
