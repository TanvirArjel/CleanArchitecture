using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using CleanHr.Blazor.Common;
using CleanHr.Blazor.Models.IdentityModels;

namespace CleanHr.Blazor.Extensions;

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
