using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Shared
{
    public partial class MainLayoutComponent
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;

        public MainLayoutComponent(ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }
        private async Task LogOutAsync()
        {
            await _localStorageService.RemoveUserInfoAsync();
            _navigationManager.NavigateTo("identity/login");
        }
    }
}
