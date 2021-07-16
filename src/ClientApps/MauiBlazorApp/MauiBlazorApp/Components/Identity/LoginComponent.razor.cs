using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MauiBlazor.Shared.Common;
using MauiBlazor.Shared.Models.IdentityModels;
using MauiBlazor.Shared.Services;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor;
using TanvirArjel.Blazor.Components;

namespace MauiBlazorApp.Components.Identity
{
    public partial class LoginComponent
    {
        private readonly UserService _userService;
        private readonly HostAuthStateProvider _hostAuthStateProvider;
        private readonly ExceptionLogger _exceptionLogger;

        public LoginComponent(
            UserService userService,
            HostAuthStateProvider hostAuthStateProvider,
            ExceptionLogger exceptionLogger)
        {
            _userService = userService;
            _hostAuthStateProvider = hostAuthStateProvider;
            _exceptionLogger = exceptionLogger;
        }

        private EditContext FormContext { get; set; }

        private LoginModel LoginModel { get; set; } = new LoginModel();

        private CustomValidationMessages ValidationMessages { get; set; }

        private bool IsSubmitBtnDisabled { get; set; }

        protected override void OnInitialized()
        {
            FormContext = new EditContext(LoginModel);
            FormContext.EnableDataAnnotationsValidation();
            FormContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
        }

        private async Task HandleValidSubmitAsync()
        {
            try
            {
                IsSubmitBtnDisabled = true;
                HttpResponseMessage httpResponse = await _userService.LoginAsync(LoginModel);

                if (httpResponse.IsSuccessStatusCode)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    string responseString = await httpResponse.Content.ReadAsStringAsync();
                    LoggedInUserInfo loginResponse = JsonSerializer.Deserialize<LoggedInUserInfo>(responseString, jsonSerializerOptions);

                    Console.WriteLine(loginResponse);

                    if (loginResponse != null)
                    {
                        await _hostAuthStateProvider.LogInAsync(loginResponse, "/");
                        return;
                    }
                }
                else
                {
                    await ValidationMessages.AddAndDisplayAsync(httpResponse);
                }
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Status Code: {httpException.StatusCode}");
                ValidationMessages.AddAndDisplay(AppErrorMessage.ServerErrorMessage);
                await _exceptionLogger.LogAsync(httpException);
            }
            catch (Exception exception)
            {
                ValidationMessages.AddAndDisplay(AppErrorMessage.ClientErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }

            IsSubmitBtnDisabled = false;
        }
    }
}
