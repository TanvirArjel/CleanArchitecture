using System.Globalization;
using System.Security.Cryptography;
using CleanHr.Application.Infrastructures;
using CleanHr.Application.Services;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class SendEmailVerificationCodeCommand(string email) : IRequest
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));
}

internal class SendEmailVerificationCodeCommandHandler : IRequestHandler<SendEmailVerificationCodeCommand>
{
    private readonly IRepository _repository;
    private readonly ViewRenderService _viewRenderService;
    private readonly IEmailSender _emailSender;

    public SendEmailVerificationCodeCommandHandler(
        IRepository repository,
        ViewRenderService viewRenderService,
        IEmailSender emailSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
    }

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

        await _repository.AddAsync(emailVerificationCode, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        (string Email, string VerificationCode) model = (request.Email, verificationCode);
        string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/ConfirmRegistrationCodeTemplate", model);

        string senderEmail = "noreply@yourapp.com";
        string subject = "User Registration";

        EmailMessage emailObject = new(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);

        await _emailSender.SendAsync(emailObject);
    }
}
