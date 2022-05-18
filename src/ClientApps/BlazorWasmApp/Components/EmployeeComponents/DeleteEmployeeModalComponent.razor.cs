using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApps.Shared.Common;
using BlazorApps.Shared.Models.EmployeeModels;
using BlazorApps.Shared.Services;
using Microsoft.AspNetCore.Components;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Utilities;

namespace BlazorWasmApp.Components.EmployeeComponents;

public partial class DeleteEmployeeModalComponent
{
    private readonly EmployeeService _employeeService;
    private readonly ExceptionLogger _exceptionLogger;

    public DeleteEmployeeModalComponent(EmployeeService employeeService, ExceptionLogger exceptionLogger)
    {
        _employeeService = employeeService;
        _exceptionLogger = exceptionLogger;
    }

    [Parameter]
    public EventCallback EmployeeDeleted { get; set; }

    private string ModalClass { get; set; } = string.Empty;

    private bool ShowBackdrop { get; set; }

    private CustomValidationMessages CustomValidationMessages { get; set; }

    private EmployeeDetailsModel EmployeeDetailsModel { get; set; } = new EmployeeDetailsModel();

    public async Task OpenAsync(Guid employeeId)
    {
        EmployeeDetailsModel = await _employeeService.GetDetailsByIdAsync(employeeId);
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
            HttpResponseMessage httpResponseMessage = await _employeeService.DeleteAsync(EmployeeDetailsModel.Id);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await EmployeeDeleted.InvokeAsync();
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
