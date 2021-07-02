using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.AccountModels
{
    public class ResendEmailConfirmationCodeModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
