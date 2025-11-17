using CleanHr.Blazor.Common;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Blazor.Extensions;

internal static class EditContextExtensions
{
    public static void AddBootstrapValidationClassProvider(this EditContext editContext)
    {
        editContext.ThrowIfNull(nameof(editContext));

        editContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
    }
}
