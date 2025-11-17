using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

public class LoginModel
{
    [Required(ErrorMessage = "Please enter your email or username.")]
    [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(50, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    [Display(Name = "Email/UserName")]
    public string EmailOrUserName { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [MinLength(6, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string Password { get; set; }
}
