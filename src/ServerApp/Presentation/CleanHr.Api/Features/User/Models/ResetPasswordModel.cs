namespace CleanHr.Api.Features.User.Models;

internal class ResetPasswordModel
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
}
