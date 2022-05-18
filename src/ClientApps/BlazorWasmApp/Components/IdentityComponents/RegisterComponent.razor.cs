using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApps.Shared.Common;
using BlazorApps.Shared.Models.IdentityModels;
using BlazorApps.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Extensions;
using TanvirArjel.Blazor.Utilities;

namespace BlazorWasmApp.Components.IdentityComponents;

public partial class RegisterComponent
{
    private readonly UserService _userService;
    private readonly NavigationManager _navigationManager;
    private readonly ExceptionLogger _exceptionLogger;

    public RegisterComponent(
        UserService userService,
        NavigationManager navigationManager,
        ExceptionLogger exceptionLogger)
    {
        _userService = userService;
        _navigationManager = navigationManager;
        _exceptionLogger = exceptionLogger;
    }

    private EditContext FormContext { get; set; }

    private RegistrationModel RegistrationModel { get; set; } = new RegistrationModel();

    private CustomValidationMessages ValidationMessages { get; set; }

    private bool IsSubmitBtnDisabled { get; set; }

    protected override void OnInitialized()
    {
        FormContext = new EditContext(RegistrationModel);
        FormContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            string error = _navigationManager.GetQuery("error");

            if (!string.IsNullOrWhiteSpace(error))
            {
                ValidationMessages.AddAndDisplay(string.Empty, error);
            }
        }
    }

    private async Task HandleValidSubmitAsync()
    {
        try
        {
            IsSubmitBtnDisabled = true;
            HttpResponseMessage httpResponseMessage = await _userService.RegisterAsync(RegistrationModel);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _navigationManager.NavigateTo("identity/login");
                return;
            }

            await ValidationMessages.AddAndDisplayAsync(httpResponseMessage);
            IsSubmitBtnDisabled = false;
        }
        catch (Exception exception)
        {
            IsSubmitBtnDisabled = false;
            ValidationMessages.AddAndDisplay(ErrorMessages.ClientErrorMessage);
            await _exceptionLogger.LogAsync(exception);
        }
    }

    private void SignUpWithGoogle()
    {
        string loginUrl = "https://localhost:44363/api/v1/external-login/sign-up?provider=Google";
        _navigationManager.NavigateTo(loginUrl, true);
    }
}
