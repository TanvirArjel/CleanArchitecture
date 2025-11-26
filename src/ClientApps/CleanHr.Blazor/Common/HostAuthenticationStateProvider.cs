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
            string accessToken = await _localStorage.GetItemAsync<string>(LocalStorageKey.Jwt);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }

            ClaimsPrincipal claimsPrincipal = _jwtTokenParser.Parse(accessToken);

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

    public async Task LogInAsync(string accessToken, string refreshToken, string redirectTo = null)
    {
        ArgumentNullException.ThrowIfNull(accessToken);

        await _localStorage.SetItemAsync(LocalStorageKey.Jwt, accessToken);

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _localStorage.SetItemAsync(LocalStorageKey.RefreshToken, refreshToken);
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        if (redirectTo != null)
        {
            _navigationManager.NavigateTo(redirectTo);
        }
    }

    // Overload for backward compatibility with external login
    public async Task LogInAsync(string jsonWebToken, string redirectTo = null)
    {
        ArgumentNullException.ThrowIfNull(jsonWebToken);
        await LogInAsync(jsonWebToken, null, redirectTo);
    }

    public async Task LogOutAsync(string redirectTo = null)
    {
        await _localStorage.RemoveItemAsync(LocalStorageKey.Jwt);
        await _localStorage.RemoveItemAsync(LocalStorageKey.RefreshToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        if (redirectTo != null)
        {
            _navigationManager.NavigateTo(redirectTo);
        }
    }
}
