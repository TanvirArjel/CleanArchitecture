using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.IdentityModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
