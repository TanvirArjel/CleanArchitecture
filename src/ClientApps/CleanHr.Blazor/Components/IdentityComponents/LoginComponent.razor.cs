using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CleanHr.Blazor.Common;
using CleanHr.Blazor.Models.IdentityModels;
using CleanHr.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;
using TanvirArjel.Blazor.Extensions;
using TanvirArjel.Blazor.Utilities;

namespace CleanHr.Blazor.Components.IdentityComponents;

public partial class LoginComponent(
    UserService userService,
    HostAuthStateProvider hostAuthStateProvider,
    ExceptionLogger exceptionLogger,
    NavigationManager navigationManager)
{
    private readonly UserService _userService = userService;
    private readonly HostAuthStateProvider _hostAuthStateProvider = hostAuthStateProvider;
    private readonly ExceptionLogger _exceptionLogger = exceptionLogger;
    private readonly NavigationManager _navigationManager = navigationManager;

    private EditContext FormContext { get; set; }

    private LoginModel LoginModel { get; set; } = new LoginModel();

    private CustomValidationMessages ValidationMessages { get; set; }

    private bool IsSubmitDisabled { get; set; }

    private string ErrorMessage { get; set; }

    protected override void OnInitialized()
    {
        FormContext = new EditContext(LoginModel);
        FormContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string error = _navigationManager.GetQuery("error");

            if (!string.IsNullOrWhiteSpace(error))
            {
                ValidationMessages.AddAndDisplay(string.Empty, error);
                return;
            }

            string jwt = _navigationManager.GetQuery("jwt");

            if (!string.IsNullOrWhiteSpace(jwt))
            {
                await _hostAuthStateProvider.LogInAsync(jwt, "/");
                return;
            }
        }
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            IsSubmitDisabled = true;
            HttpResponseMessage httpResponse = await _userService.LoginAsync(LoginModel);

            if (httpResponse.IsSuccessStatusCode)
            {
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                string jsonWebToken = JsonSerializer.Deserialize<string>(responseString);

                Console.WriteLine(jsonWebToken);

                if (jsonWebToken != null)
                {
                    await _hostAuthStateProvider.LogInAsync(jsonWebToken, "/");
                    return;
                }
            }
            else
            {
                Console.WriteLine((int)httpResponse.StatusCode);
                await ValidationMessages.AddAndDisplayAsync(httpResponse);
            }
        }
        catch (HttpRequestException httpException)
        {
            Console.WriteLine($"Status Code: {httpException.StatusCode}");
            ValidationMessages.AddAndDisplay(ErrorMessages.Http500ErrorMessage);
            await _exceptionLogger.LogAsync(httpException);
        }
        catch (Exception exception)
        {
            ValidationMessages.AddAndDisplay(ErrorMessages.ClientErrorMessage);
            await _exceptionLogger.LogAsync(exception);
        }

        IsSubmitDisabled = false;
    }

    private void LoginWithGoogle()
    {
        string loginUrl = "https://localhost:44363/api/v1/external-login/sign-in?provider=Google";
        _navigationManager.NavigateTo(loginUrl, true);
    }
}
