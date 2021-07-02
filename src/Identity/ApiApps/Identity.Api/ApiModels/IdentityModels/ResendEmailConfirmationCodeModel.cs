using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.IdentityModels
{
    public class ResendEmailConfirmationCodeModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
