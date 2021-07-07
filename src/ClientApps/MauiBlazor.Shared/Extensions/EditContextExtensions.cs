using MauiBlazor.Shared.Common;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.ArgumentChecker;

namespace MauiBlazor.Shared.Extensions
{
    public static class EditContextExtensions
    {
        public static void AddBootstrapValidationClassProvider(this EditContext editContext)
        {
            editContext.ThrowIfNull(nameof(editContext));

            editContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
        }
    }
}
