using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        public HostAuthStateProvider(ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
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

                Console.WriteLine(jwtToken);

                List<Claim> microsoftClaims = new List<Claim>();

                foreach (Claim item in jwtToken.Claims)
                {
                    switch (item.Type)
                    {
                        case JwtRegisteredClaimNames.NameId:
                            microsoftClaims.Add(new Claim(ClaimTypes.NameIdentifier, item.Value));
                            break;
                        case JwtRegisteredClaimNames.Name:
                            microsoftClaims.Add(new Claim(ClaimTypes.Name, item.Value));
                            break;
                        case JwtRegisteredClaimNames.GivenName:
                            microsoftClaims.Add(new Claim(ClaimTypes.GivenName, item.Value));
                            break;
                        case JwtRegisteredClaimNames.Sub:
                            microsoftClaims.Add(new Claim(ClaimTypes.NameIdentifier, item.Value));
                            break;
                        case JwtRegisteredClaimNames.Email:
                            microsoftClaims.Add(new Claim(ClaimTypes.Email, item.Value));
                            break;
                        case JwtRegisteredClaimNames.Iat:
                            microsoftClaims.Add(new Claim(ClaimTypes.Expiration, item.Value));
                            break;
                        case JwtRegisteredClaimNames.Jti:
                            microsoftClaims.Add(new Claim(ClaimTypes.Sid, item.Value));
                            break;
                        default:
                            break;
                    }
                }


                ClaimsIdentity identity = new ClaimsIdentity(microsoftClaims, "ServerAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                AuthenticationState authenticationState = new AuthenticationState(claimsPrincipal);

                return authenticationState;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        public async Task LogInAsync(LoggedInUserInfo loggedInUserInfo, string redirectTo = null)
        {
            if (loggedInUserInfo == null)
            {
                throw new ArgumentNullException(nameof(loggedInUserInfo));
            }

            await _localStorage.SetItemAsync(LocalStorageKey.LoggedInUserInfo, loggedInUserInfo);
            await NotifyStatusChanged();

            if (redirectTo != null)
            {
                _navigationManager.NavigateTo(redirectTo);
            }
        }

        public async Task LogOutAsync(string redirectTo = null)
        {
            await _localStorage.RemoveItemAsync(LocalStorageKey.LoggedInUserInfo);
            await NotifyStatusChanged();

            if (redirectTo != null)
            {
                _navigationManager.NavigateTo(redirectTo);
            }
        }

        private async Task NotifyStatusChanged()
        {
            AuthenticationState authenticationState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
    }
}
