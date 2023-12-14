using System;
using System.Net.Http;
using System.Threading.Tasks;
using CleanHr.Blazor.Common;
using CleanHr.Blazor.Extensions;
using CleanHr.Blazor.Models.DepartmentModels;
using CleanHr.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Utilities;

namespace CleanHr.Blazor.Components.DepartmentComponents;

public partial class UpdateDepartmentModalComponent(
    DepartmentService departmentService,
    ExceptionLogger exceptionLogger)
{
    private readonly DepartmentService _departmentService = departmentService;
    private readonly ExceptionLogger _exceptionLogger = exceptionLogger;

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
