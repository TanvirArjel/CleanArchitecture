using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress]
    public string Email { get; set; }
}
