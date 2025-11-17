using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

public class ChangePasswordModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Please enter your current password.")]
    [MinLength(8, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Please enter your new password.")]
    [MinLength(8, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your new password.")]
    [Compare("Password")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; }
}
