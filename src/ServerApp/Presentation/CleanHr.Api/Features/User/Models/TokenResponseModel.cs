namespace CleanHr.Api.Features.User.Models;

public sealed class TokenResponseModel
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}
