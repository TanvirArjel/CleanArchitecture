using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

public class RegistrationModel
{
    [Required(ErrorMessage = "Please enter your first name.")]
    [MaxLength(30, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Please enter your last name.")]
    [MaxLength(30, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Please enter your email address.")]
    [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(50, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [MinLength(6, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please enter your confirm password.")]
    [Compare(nameof(Password), ErrorMessage = "Confirm password does match with password.")]
    public string ConfirmPassword { get; set; }
}
