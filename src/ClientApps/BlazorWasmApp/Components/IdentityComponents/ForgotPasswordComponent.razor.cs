using System.Threading.Tasks;
using BlazorApps.Shared.Common;
using BlazorApps.Shared.Models.IdentityModels;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;

namespace BlazorWasmApp.Components.IdentityComponents
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
            FormContext.SetFieldCssClassProvider(new BootstrapValidationClassProvider());
        }

        private async Task HandleValidSubmitAsync()
        {
            await Task.CompletedTask;
        }
    }
}
