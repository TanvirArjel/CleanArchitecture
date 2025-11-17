namespace CleanHr.Api.Features.User.Models;

internal class LoginModel
{
    public string EmailOrUserName { get; set; }

    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
