using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.AccountModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
