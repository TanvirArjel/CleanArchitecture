using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmApp.Common;
using BlazorWasmApp.Models;
using BlazorWasmApp.Models.EmployeeModels;
using BlazorWasmApp.Services;
using Microsoft.AspNetCore.Components;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Utilities;

namespace BlazorWasmApp.Components.EmployeeComponents;

public partial class UpdateEmployeeModalComponent
{
    private readonly EmployeeService _employeeService;
    private readonly DepartmentService _departmentService;
    private readonly ExceptionLogger _exceptionLogger;

    public UpdateEmployeeModalComponent(
        EmployeeService employeeService,
        DepartmentService departmentService,
        ExceptionLogger exceptionLogger)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _exceptionLogger = exceptionLogger;
    }

    [Parameter]
    public EventCallback EmployeeUpdated { get; set; }

    private string ModalClass { get; set; } = string.Empty;

    private bool ShowBackdrop { get; set; }

    private CustomValidationMessages CustomValidationMessages { get; set; }

    private UpdateEmployeeModel UpdateEmployeeModel { get; set; }

    private List<SelectListItem> DepartmentSelectList { get; set; }

    private string ErrorMessage { get; set; }

    public async Task OpenAsync(Guid employeeId)
    {
        EmployeeDetailsModel employeeDetailsViewModel = await _employeeService.GetDetailsByIdAsync(employeeId);

        if (employeeDetailsViewModel == null)
        {
            ErrorMessage = "Employee with provided id does not exists!";
        }

        UpdateEmployeeModel = new UpdateEmployeeModel()
        {
            Id = employeeDetailsViewModel.Id,
            Name = employeeDetailsViewModel.Name,
            DepartmentId = employeeDetailsViewModel.DepartmentId,
            DateOfBirth = employeeDetailsViewModel.DateOfBirth,
            Email = employeeDetailsViewModel.Email,
            PhoneNumber = employeeDetailsViewModel.PhoneNumber
        };

        DepartmentSelectList = await _departmentService.GetSelectListAsync(employeeDetailsViewModel.DepartmentId);

        ModalClass = "show d-block";
        ShowBackdrop = true;
        StateHasChanged();
    }

    private void Close()
    {
        UpdateEmployeeModel = new UpdateEmployeeModel();
        ModalClass = string.Empty;
        ShowBackdrop = false;
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            HttpResponseMessage httpResponseMessage = await _employeeService.UpdateAsync(UpdateEmployeeModel);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await EmployeeUpdated.InvokeAsync();
                Close();
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
