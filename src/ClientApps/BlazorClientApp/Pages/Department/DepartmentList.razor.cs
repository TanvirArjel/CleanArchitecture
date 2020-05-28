using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorClientApp.Pages.Department
{
    public partial class DepartmentList
    {
        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        private List<DepartmentDetailsViewModel> Departments { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Departments = await DepartmentService.GetDepartmentListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persists please contact with system administrator.";
            }
        }
    }
}
