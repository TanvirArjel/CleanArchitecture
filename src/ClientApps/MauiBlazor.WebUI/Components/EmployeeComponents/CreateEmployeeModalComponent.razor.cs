using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MauiBlazor.Shared.Common;
using MauiBlazor.Shared.Models;
using MauiBlazor.Shared.Models.EmployeeModels;
using MauiBlazor.Shared.Services;
using Microsoft.AspNetCore.Components;
using TanvirArjel.Blazor;
using TanvirArjel.Blazor.Components;

namespace MauiBlazor.WebUI.Components.EmployeeComponents
{
    public partial class CreateEmployeeModalComponent
    {
        private readonly EmployeeService _employeeService;
        private readonly DepartmentService _departmentService;
        private readonly ExceptionLogger _exceptionLogger;

        public CreateEmployeeModalComponent(
            EmployeeService employeeService,
            DepartmentService departmentService,
            ExceptionLogger exceptionLogger)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _exceptionLogger = exceptionLogger;
        }

        private string ModalClass { get; set; } = string.Empty;

        private bool ShowBackdrop { get; set; }

        private CustomValidationMessages CustomValidationMessages { get; set; }

        private CreateEmployeeModel CreateEmployeeViewModel { get; set; } = new CreateEmployeeModel();

        private List<SelectListItem> DepartmentSelectList { get; set; } = new List<SelectListItem>();

        [Parameter]
        public EventCallback EmployeeCreated { get; set; }

        public async Task OpenAsync()
        {
            CreateEmployeeViewModel = new CreateEmployeeModel();
            List<SelectListItem> items = await _departmentService.GetSelectListAsync();
            DepartmentSelectList = items;

            ModalClass = "show d-block";
            ShowBackdrop = true;
            StateHasChanged();
        }

        private void Close()
        {
            ModalClass = string.Empty;
            ShowBackdrop = false;
            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _employeeService.CreateAsync(CreateEmployeeViewModel);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Close();
                    await EmployeeCreated.InvokeAsync();
                    return;
                }

                await CustomValidationMessages.AddAndDisplayAsync(httpResponseMessage);

            }
            catch (Exception exception)
            {
                CustomValidationMessages.AddAndDisplay(string.Empty, AppErrorMessage.ClientErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }
        }
    }
}
