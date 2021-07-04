using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Common;
using BlazorWasmApp.ViewModels.IdentityModels;

namespace BlazorWasmApp.Extensions
{
    public static class LocalStorageServiceExtensions
    {
        public static async Task<LoggedInUserInfo> GetUserInfoAsync(this ILocalStorageService localStorage)
        {
            if (localStorage == null)
            {
                throw new ArgumentNullException(nameof(localStorage));
            }

            LoggedInUserInfo loggedInUserInfo = await localStorage.GetItemAsync<LoggedInUserInfo>(LocalStorageKey.LoggedInUserInfo);

            return loggedInUserInfo;
        }
    }
}
