using System.Globalization;
using System.Security.Cryptography;
using CleanHr.Application.Infrastructures;
using CleanHr.Application.Services;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class SendEmailVerificationCodeCommand : IRequest
{
    public SendEmailVerificationCodeCommand(string email)
    {
        Email = email.ThrowIfNotValidEmail(nameof(email));
    }

    public string Email { get; }
}

internal class SendEmailVerificationCodeCommandHandler(
    IRepository repository,
    ViewRenderService viewRenderService,
    IEmailSender emailSender) : IRequestHandler<SendEmailVerificationCodeCommand>
{
    public async Task Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        int randomNumber = RandomNumberGenerator.GetInt32(0, 1000000);
        string verificationCode = randomNumber.ToString("D6", CultureInfo.InvariantCulture);

        EmailVerificationCode emailVerificationCode = new()
        {
            Code = verificationCode,
            Email = request.Email,
            SentAtUtc = DateTime.UtcNow
        };

        await repository.AddAsync(emailVerificationCode, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        (string Email, string VerificationCode) model = (request.Email, verificationCode);
        string emailBody = await viewRenderService.RenderViewToStringAsync("EmailTemplates/ConfirmRegistrationCodeTemplate", model);

        string senderEmail = "noreply@yourapp.com";
        string subject = "User Registration";

        EmailMessage emailObject = new(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);

        await emailSender.SendAsync(emailObject);
    }
}
