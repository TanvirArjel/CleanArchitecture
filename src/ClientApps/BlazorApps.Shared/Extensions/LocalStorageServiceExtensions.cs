using System;
using System.Threading.Tasks;
using BlazorApps.Shared.Common;
using BlazorApps.Shared.Models.IdentityModels;
using Blazored.LocalStorage;

namespace BlazorApps.Shared.Extensions;

public static class LocalStorageServiceExtensions
{
    public static async Task<LoggedInUserInfo> GetUserInfoAsync(this ILocalStorageService localStorage)
    {
        if (localStorage == null)
        {
            throw new ArgumentNullException(nameof(localStorage));
        }

        LoggedInUserInfo loggedInUserInfo = await localStorage.GetItemAsync<LoggedInUserInfo>(LocalStorageKey.Jwt);

        return loggedInUserInfo;
    }
}
