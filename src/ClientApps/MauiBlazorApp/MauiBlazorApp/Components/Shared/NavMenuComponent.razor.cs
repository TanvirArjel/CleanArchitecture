using System.Threading.Tasks;
using MauiBlazor.Shared.Common;

namespace MauiBlazorApp.Components.Shared
{
    public partial class NavMenuComponent
    {
        private readonly HostAuthStateProvider _hostAuthStateProvider;

        public NavMenuComponent(HostAuthStateProvider hostAuthStateProvider)
        {
            _hostAuthStateProvider = hostAuthStateProvider;
        }

        private bool IsNavMenuOpen { get; set; }

        private string NavMenuCssClass => IsNavMenuOpen ? "open" : null;

        private void ToggleNavMenu()
        {
            IsNavMenuOpen = !IsNavMenuOpen;
        }

        private async Task LogOutAsync()
        {
            await _hostAuthStateProvider.LogOutAsync("/");
        }
    }
}
