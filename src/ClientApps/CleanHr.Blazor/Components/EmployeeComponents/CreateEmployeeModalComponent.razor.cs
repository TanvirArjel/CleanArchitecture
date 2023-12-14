using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CleanHr.Blazor.Common;
using CleanHr.Blazor.Models;
using CleanHr.Blazor.Models.EmployeeModels;
using CleanHr.Blazor.Services;
using Microsoft.AspNetCore.Components;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Utilities;

namespace CleanHr.Blazor.Components.EmployeeComponents;

public partial class CreateEmployeeModalComponent(
    EmployeeService employeeService,
    DepartmentService departmentService,
    ExceptionLogger exceptionLogger)
{
    private readonly EmployeeService _employeeService = employeeService;
    private readonly DepartmentService _departmentService = departmentService;
    private readonly ExceptionLogger _exceptionLogger = exceptionLogger;

    [Parameter]
    public EventCallback EmployeeCreated { get; set; }

    private string ModalClass { get; set; } = string.Empty;

    private bool ShowBackdrop { get; set; }

    private CustomValidationMessages CustomValidationMessages { get; set; }

    private CreateEmployeeModel CreateEmployeeViewModel { get; set; } = new CreateEmployeeModel();

    private List<SelectListItem> DepartmentSelectList { get; set; } = new List<SelectListItem>();

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
            CustomValidationMessages.AddAndDisplay(string.Empty, ErrorMessages.ClientErrorMessage);
            await _exceptionLogger.LogAsync(exception);
        }
    }
}
