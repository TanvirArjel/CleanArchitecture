using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

public class SetUserPasswordModel
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; }
}
