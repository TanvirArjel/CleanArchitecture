using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.IdentityModels
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "{0} should be between {2} to {1} characters")]
        public string EmailOrUserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
