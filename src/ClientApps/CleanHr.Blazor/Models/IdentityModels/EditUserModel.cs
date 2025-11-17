using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.IdentityModels;

public class EditUserModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MinLength(5, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [RegularExpression(@"^[a-zA-Z,\s]+$", ErrorMessage = "The {0} should contain only letters.")]
    public string FullName { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]+$", ErrorMessage = "The {0} should contain only letters and digits.")]
    [MinLength(3, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {0} should not be more than {1} characters long.")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public bool IsActive { get; set; }

    public Dictionary<int, string> Roles { get; set; }
}
