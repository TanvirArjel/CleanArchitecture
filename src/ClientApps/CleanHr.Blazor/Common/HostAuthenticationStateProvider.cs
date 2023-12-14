using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanHr.Blazor.Common;

public class HostAuthStateProvider(
    ILocalStorageService localStorage,
    NavigationManager navigationManager,
    JwtTokenParser jwtTokenParser) : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly JwtTokenParser _jwtTokenParser = jwtTokenParser;

    public ClaimsPrincipal User { get; private set; }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string jsonWebToken = await _localStorage.GetItemAsync<string>(LocalStorageKey.Jwt);

            if (jsonWebToken == null)
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }

            ClaimsPrincipal claimsPrincipal = _jwtTokenParser.Parse(jsonWebToken);

            AuthenticationState authenticationState = new(claimsPrincipal);
            User = authenticationState.User;

            return authenticationState;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            AuthenticationState authState = new(new ClaimsPrincipal());
            User = authState.User;
            return authState;
        }
    }

    public async Task LogInAsync(string jsonWebToken, string redirectTo = null)
    {
        ArgumentNullException.ThrowIfNull(jsonWebToken);

        await _localStorage.SetItemAsync(LocalStorageKey.Jwt, jsonWebToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        if (redirectTo != null)
        {
            _navigationManager.NavigateTo(redirectTo);
        }
    }

    public async Task LogOutAsync(string redirectTo = null)
    {
        await _localStorage.RemoveItemAsync(LocalStorageKey.Jwt);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        if (redirectTo != null)
        {
            _navigationManager.NavigateTo(redirectTo);
        }
    }
}
