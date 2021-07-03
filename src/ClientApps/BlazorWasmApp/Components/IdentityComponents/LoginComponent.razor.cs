using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Common;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.IdentityModels;
using Microsoft.AspNetCore.Components;
using TanvirArjel.Blazor.Components;

namespace BlazorWasmApp.Components.IdentityComponents
{
    public partial class LoginComponent
    {
        private readonly UserService _userService;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly ExceptionLogger _exceptionLogger;

        public LoginComponent(
            UserService userService,
            ILocalStorageService localStorage,
            NavigationManager navigationManager,
            ExceptionLogger exceptionLogger)
        {
            _userService = userService;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _exceptionLogger = exceptionLogger;
        }

        private LoginModel LoginModel { get; set; }

        private CustomValidationMessages ValidationMessages { get; set; }

        protected override void OnInitialized()
        {
            LoginModel = new LoginModel();
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                HttpResponseMessage httpResponse = await _userService.LoginAsync(LoginModel);

                if (httpResponse.IsSuccessStatusCode)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    string responseString = await httpResponse.Content.ReadAsStringAsync();
                    LoggedInUserInfo loginResponse = JsonSerializer.Deserialize<LoggedInUserInfo>(responseString, jsonSerializerOptions);

                    if (loginResponse != null)
                    {
                        await _localStorage.StoreUserInfoAsync(loginResponse);

                        _navigationManager.NavigateTo("/", true);
                    }
                }
                else
                {
                    await ValidationMessages.AddAndDisplayAsync(httpResponse);
                }
            }
            catch (Exception exception)
            {
                ValidationMessages.AddAndDisplay(AppErrorMessage.ClientErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }
        }
    }
}
