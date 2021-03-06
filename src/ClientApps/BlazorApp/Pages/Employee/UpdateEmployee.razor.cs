﻿using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlazorApp.Pages.Employee
{
    public partial class UpdateEmployee
    {
        [Parameter]
        public int EmployeeId { get; set; }

        [Inject]
        private IEmployeeService EmployeeService { get; set; }

        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private UpdateEmployeeViewModel UpdateEmployeeModel { get; set; }

        private SelectList DepartmentSelectList { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsViewModel employeeDetailsViewModel = await EmployeeService.GetEmployeeDetailsAsync(EmployeeId);

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

            DepartmentSelectList = await DepartmentService.GetDepartmentSelectListAsync(employeeDetailsViewModel.DepartmentId);
        }

        private async Task HandleValidSubmit()
        {
            await EmployeeService.UpdateEmplyeeAsync(UpdateEmployeeModel);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
