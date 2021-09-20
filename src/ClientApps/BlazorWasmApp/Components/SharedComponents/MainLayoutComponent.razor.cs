using System.Threading.Tasks;
using BlazorApps.Shared.Common;

namespace BlazorWasmApp.Components.SharedComponents
{
    public partial class MainLayoutComponent
    {
        private readonly HostAuthStateProvider _hostAuthStateProvider;

        public MainLayoutComponent(HostAuthStateProvider hostAuthStateProvider)
        {
            _hostAuthStateProvider = hostAuthStateProvider;
        }

        private async Task LogOutAsync()
        {
            await _hostAuthStateProvider.LogOutAsync("identity/login");
        }
    }
}
