using System.Threading.Tasks;
using MauiBlazor.Shared.Common;
using MauiBlazor.Shared.Models.IdentityModels;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;

namespace MauiBlazorApp.Components.Identity
{
    public partial class ForgotPasswordComponent
    {
        private EditContext FormContext { get; set; }

        private ForgotPasswordModel ForgotPasswordModel { get; set; } = new ForgotPasswordModel();

        private CustomValidationMessages ValidationMessages { get; set; }

        private bool IsSubmitBtnDisabled { get; set; }

        protected override void OnInitialized()
        {
            FormContext = new EditContext(ForgotPasswordModel);
            FormContext.EnableDataAnnotationsValidation();
            FormContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
        }

        private async Task HandleValidSubmitAsync()
        {
            await Task.CompletedTask;
        }
    }
}
