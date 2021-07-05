using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.ViewModels.IdentityModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasmApp.Common
{
    public class HostAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly JwtTokenParser _jwtTokenParser;


        public HostAuthStateProvider(
            ILocalStorageService localStorage,
            NavigationManager navigationManager,
            JwtTokenParser jwtTokenParser)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _jwtTokenParser = jwtTokenParser;
        }

        public ClaimsPrincipal User { get; private set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                LoggedInUserInfo loggedInUserInfo = await _localStorage.GetUserInfoAsync();

                if (loggedInUserInfo == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal());
                }

                ClaimsPrincipal claimsPrincipal = _jwtTokenParser.Parse(loggedInUserInfo.AccessToken);

                AuthenticationState authenticationState = new AuthenticationState(claimsPrincipal);
                User = authenticationState.User;

                return authenticationState;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                AuthenticationState authState = new AuthenticationState(new ClaimsPrincipal());
                User = authState.User;
                return authState;
            }
        }

        public async Task LogInAsync(LoggedInUserInfo loggedInUserInfo, string redirectTo = null)
        {
            if (loggedInUserInfo == null)
            {
                throw new ArgumentNullException(nameof(loggedInUserInfo));
            }

            await _localStorage.SetItemAsync(LocalStorageKey.LoggedInUserInfo, loggedInUserInfo);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            if (redirectTo != null)
            {
                _navigationManager.NavigateTo(redirectTo);
            }
        }

        public async Task LogOutAsync(string redirectTo = null)
        {
            await _localStorage.RemoveItemAsync(LocalStorageKey.LoggedInUserInfo);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            if (redirectTo != null)
            {
                _navigationManager.NavigateTo(redirectTo);
            }
        }
    }
}
