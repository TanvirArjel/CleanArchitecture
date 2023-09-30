namespace CleanHr.Api.Features.User.Models;

public class ResetPasswordModel
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
}
