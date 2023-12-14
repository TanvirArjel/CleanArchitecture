using System.Globalization;
using System.Security.Cryptography;
using CleanHr.Application.Infrastructures;
using CleanHr.Application.Services;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class SendPasswordResetCodeCommand(string email) : IRequest
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));
}

internal class SendPasswordResetCodeCommandHandler(
        IRepository repository,
        ViewRenderService viewRenderService,
        IEmailSender emailSender) : IRequestHandler<SendPasswordResetCodeCommand>
{
    public async Task Handle(SendPasswordResetCodeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        bool isExistent = await repository.ExistsAsync<ApplicationUser>(u => u.Email == request.Email, cancellationToken);

        if (isExistent == false)
        {
            throw new InvalidOperationException("The user does not exist with the provided email.");
        }

        int randomNumber = RandomNumberGenerator.GetInt32(0, 1000000);
        string verificationCode = randomNumber.ToString("D6", CultureInfo.InvariantCulture);

        PasswordResetCode emailVerificationCode = new()
        {
            Code = verificationCode,
            Email = request.Email,
            SentAtUtc = DateTime.UtcNow
        };

        await repository.AddAsync(emailVerificationCode, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        (string Email, string VerificationCode) model = (request.Email, verificationCode);
        string subject = "Reset Password";
        string senderEmail = "noreply@yourapp.com";
        string emailBody = await viewRenderService.RenderViewToStringAsync("EmailTemplates/PasswordResetCodeTemplate", model);
        EmailMessage emailObject = new(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);
        await emailSender.SendAsync(emailObject);
    }
}
