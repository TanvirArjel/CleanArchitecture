namespace CleanHr.Api.Features.User.Models;

internal class TokenRefreshModel
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}
