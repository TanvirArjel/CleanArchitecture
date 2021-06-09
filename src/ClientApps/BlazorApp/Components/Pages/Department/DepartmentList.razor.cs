using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;

namespace BlazorApp.Components.Pages.Department
{
    public partial class DepartmentList
    {
        private readonly DepartmentService _departmentService;

        public DepartmentList(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        private List<DepartmentDetailsViewModel> Departments { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Departments = await _departmentService.GetListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persists please contact with system administrator.";
            }
        }
    }
}
