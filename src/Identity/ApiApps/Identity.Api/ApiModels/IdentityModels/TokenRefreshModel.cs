using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ApiModels.IdentityModels
{
    public class TokenRefreshModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
