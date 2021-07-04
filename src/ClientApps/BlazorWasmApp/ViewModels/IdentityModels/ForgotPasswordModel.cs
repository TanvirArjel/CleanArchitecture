using System.ComponentModel.DataAnnotations;

namespace BlazorWasmApp.ViewModels.IdentityModels
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
