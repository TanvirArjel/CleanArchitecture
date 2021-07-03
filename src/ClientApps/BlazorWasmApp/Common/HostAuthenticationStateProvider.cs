using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.ViewModels.IdentityModels;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasmApp.Common
{
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public HostAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                LoggedInUserInfo loggedInUserInfo = await _localStorage.GetUserInfoAsync();

                if (loggedInUserInfo == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal());
                }

                JwtSecurityToken jwtToken = new JwtSecurityToken(loggedInUserInfo.AccessToken);

                ClaimsIdentity identity = new ClaimsIdentity(jwtToken.Claims, "ServerAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                AuthenticationState authenticationState = new AuthenticationState(claimsPrincipal);

                return await Task.FromResult(authenticationState);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }
    }
}
