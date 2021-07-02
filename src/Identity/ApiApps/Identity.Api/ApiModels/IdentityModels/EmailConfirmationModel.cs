using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.IdentityModels
{
    public class EmailConfirmationModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "{0} should be {1} characters long.")]
        [MaxLength(6, ErrorMessage = "{0} should be {1} characters long.")]
        public string Code { get; set; }
    }
}
