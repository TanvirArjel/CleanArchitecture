using System.Threading.Tasks;
using MauiBlazor.Shared.Common;

namespace MauiBlazor.WebUI.Components.SharedComponents
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
