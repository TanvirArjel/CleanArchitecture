using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlazorApp.Pages.Employee
{
    public partial class UpdateEmployee
    {
        private readonly EmployeeService _employeeService;
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public UpdateEmployee(
            EmployeeService employeeService,
            DepartmentService departmentService,
            NavigationManager navigationManager)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _navigationManager = navigationManager;
        }

        [Parameter]
        public int EmployeeId { get; set; }

        private UpdateEmployeeViewModel UpdateEmployeeModel { get; set; }

        private List<SelectListItem> DepartmentSelectList { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsViewModel employeeDetailsViewModel = await _employeeService.GetDetailsByIdAsync(EmployeeId);

            if (employeeDetailsViewModel == null)
            {
                ErrorMessage = "Employee with provided id does not exists!";
            }

            UpdateEmployeeModel = new UpdateEmployeeViewModel()
            {
                EmployeeId = employeeDetailsViewModel.EmployeeId,
                EmployeeName = employeeDetailsViewModel.EmployeeName,
                DepartmentId = employeeDetailsViewModel.DepartmentId,
                DateOfBirth = employeeDetailsViewModel.DateOfBirth,
                Email = employeeDetailsViewModel.Email,
                PhoneNumber = employeeDetailsViewModel.PhoneNumber
            };

            DepartmentSelectList = await _departmentService.GetSelectListAsync(employeeDetailsViewModel.DepartmentId);
        }

        private async Task HandleValidSubmit()
        {
            await _employeeService.UpdateAsync(UpdateEmployeeModel);
            _navigationManager.NavigateTo("employee/employee-list");
        }
    }
}
