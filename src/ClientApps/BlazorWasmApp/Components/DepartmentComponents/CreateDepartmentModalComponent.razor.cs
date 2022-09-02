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

public partial class CreateDepartmentModalComponent
{
    private readonly DepartmentService _departmentService;
    private readonly ExceptionLogger _exceptionLogger;

    public CreateDepartmentModalComponent(DepartmentService departmentService, ExceptionLogger exceptionLogger)
    {
        _departmentService = departmentService;
        _exceptionLogger = exceptionLogger;
    }

    [Parameter]
    public EventCallback DepartmentCreated { get; set; }

    private string ModalClass { get; set; } = string.Empty;

    private bool ShowBackdrop { get; set; }

    private CustomValidationMessages CustomValidationMessages { get; set; }

    private EditContext FormEditContext { get; set; }

    private CreateDepartmentModel CreateDepartmentModel { get; set; } = new CreateDepartmentModel();

    public void Show()
    {
        FormEditContext = new EditContext(CreateDepartmentModel);

        ModalClass = "show d-block";
        ShowBackdrop = true;
        StateHasChanged();
    }

    public async Task HandleValidSubmit()
    {
        try
        {
            HttpResponseMessage httpResponseMessage = await _departmentService.CreateAsync(CreateDepartmentModel);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Close();
                await DepartmentCreated.InvokeAsync();
                return;
            }

            await CustomValidationMessages.AddAndDisplayAsync(httpResponseMessage);
        }
        catch (HttpRequestException exception)
        {
            CustomValidationMessages.AddAndDisplay(ErrorMessages.ServerDownOrCorsErrorMessage);
            await _exceptionLogger.LogAsync(exception);
        }
        catch (Exception exception)
        {
            CustomValidationMessages.AddAndDisplay(ErrorMessages.ClientErrorMessage);
            await _exceptionLogger.LogAsync(exception);
        }
    }

    protected override void OnInitialized()
    {
        FormEditContext = new EditContext(CreateDepartmentModel);
        FormEditContext.AddBootstrapValidationClassProvider();
    }

    private void Close()
    {
        CreateDepartmentModel = new CreateDepartmentModel();
        ModalClass = string.Empty;
        ShowBackdrop = false;
        StateHasChanged();
    }
}
