using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.Utils;
using BlazorWasmApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.EmployeeComponents
{
    public partial class CreateEmployeeComponent
    {
        private readonly EmployeeService _employeeService;
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public CreateEmployeeComponent(
            EmployeeService employeeService,
            DepartmentService departmentService,
            NavigationManager navigationManager)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _navigationManager = navigationManager;
        }

        private CreateEmployeeViewModel CreateEmployeeViewModel { get; set; } = new CreateEmployeeViewModel();

        private List<SelectListItem> DepartmentSelectList { get; set; } = new List<SelectListItem>();

        protected override async Task OnInitializedAsync()
        {
            List<SelectListItem> items = await _departmentService.GetSelectListAsync();
            DepartmentSelectList = items;
        }

        private async Task HandleValidSubmit()
        {
            await _employeeService.CreateAsync(CreateEmployeeViewModel);
            _navigationManager.NavigateTo("employee/employee-list");
        }
    }
}
