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

public partial class CreateDepartmentModalComponent(
    DepartmentService departmentService,
    ExceptionLogger exceptionLogger)
{
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
            HttpResponseMessage httpResponseMessage = await departmentService.CreateAsync(CreateDepartmentModel);

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
            await exceptionLogger.LogAsync(exception);
        }
        catch (Exception exception)
        {
            CustomValidationMessages.AddAndDisplay(ErrorMessages.ClientErrorMessage);
            await exceptionLogger.LogAsync(exception);
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
