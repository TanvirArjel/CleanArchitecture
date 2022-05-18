using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApps.Shared.Common;
using BlazorApps.Shared.Extensions;
using BlazorApps.Shared.Models.DepartmentModels;
using BlazorApps.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Utilities;

namespace BlazorWasmApp.Components.DepartmentComponents;

public partial class UpdateDepartmentModalComponent
{
    private readonly DepartmentService _departmentService;
    private readonly ExceptionLogger _exceptionLogger;

    public UpdateDepartmentModalComponent(DepartmentService departmentService, ExceptionLogger exceptionLogger)
    {
        _departmentService = departmentService;
        _exceptionLogger = exceptionLogger;
    }

    [Parameter]
    public EventCallback DepartmentUpdated { get; set; }

    private string ModalClass { get; set; } = string.Empty;

    private bool ShowBackdrop { get; set; }

    private CustomValidationMessages CustomValidationMessages { get; set; }

    private EditContext FormEditContext { get; set; }

    private UpdateDepartmentModel UpdateDepartmentModel { get; set; } = new UpdateDepartmentModel();

    private string ErrorMessage { get; set; }

    public async Task ShowAsync(Guid departmentId)
    {
        DepartmentDetailsModel departmentDetailsViewModel = await _departmentService.GetByIdAsync(departmentId);

        if (departmentDetailsViewModel == null)
        {
            ErrorMessage = "Employee with provided id does not exists!";
        }

        UpdateDepartmentModel = new UpdateDepartmentModel()
        {
            Id = departmentDetailsViewModel.Id,
            Name = departmentDetailsViewModel.Name,
            Description = departmentDetailsViewModel.Description,
        };

        FormEditContext = new EditContext(UpdateDepartmentModel);
        FormEditContext.AddBootstrapValidationClassProvider();

        ModalClass = "show d-block";
        ShowBackdrop = true;
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        FormEditContext = new EditContext(UpdateDepartmentModel);
    }

    private void Close()
    {
        UpdateDepartmentModel = new UpdateDepartmentModel();
        ModalClass = string.Empty;
        ShowBackdrop = false;
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            HttpResponseMessage httpResponseMessage = await _departmentService.UpdateAsync(UpdateDepartmentModel);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Close();
                await DepartmentUpdated.InvokeAsync();
                return;
            }

            await CustomValidationMessages.AddAndDisplayAsync(httpResponseMessage);
        }
        catch (Exception exception)
        {
            await _exceptionLogger.LogAsync(exception);
            CustomValidationMessages.AddAndDisplay(string.Empty, ErrorMessages.ClientErrorMessage);
        }
    }
}
