using System.Threading.Tasks;
using CleanHr.Blazor.Common;
using CleanHr.Blazor.Models.IdentityModels;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;

namespace CleanHr.Blazor.Components.IdentityComponents;

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
