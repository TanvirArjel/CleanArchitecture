using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

internal sealed class ResetPasswordModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "{0} should be exactly {1} characters long.")]
    public string Code { get; set; }
}
