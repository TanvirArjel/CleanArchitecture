using System.Globalization;
using System.Security.Cryptography;
using CleanHr.Application.Infrastructures;
using CleanHr.Application.Services;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class SendPasswordResetCodeCommand(string email) : IRequest<Result>
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));
}

internal class SendPasswordResetCodeCommandHandler : IRequestHandler<SendPasswordResetCodeCommand, Result>
{
    private readonly IRepository _repository;
    private readonly ViewRenderService _viewRenderService;
    private readonly IEmailSender _emailSender;

    public SendPasswordResetCodeCommandHandler(
            IRepository repository,
            ViewRenderService viewRenderService,
            IEmailSender emailSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
    }

    public async Task<Result> Handle(SendPasswordResetCodeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        bool isExistent = await _repository.ExistsAsync<ApplicationUser>(u => u.Email == request.Email, cancellationToken);

        if (isExistent == false)
        {
            return Result.Failure("The user does not exist with the provided email.");
        }

        int randomNumber = RandomNumberGenerator.GetInt32(0, 1000000);
        string verificationCode = randomNumber.ToString("D6", CultureInfo.InvariantCulture);

        Result<PasswordResetCode> result = await PasswordResetCode.CreateAsync(request.Email, verificationCode);

        if (result.IsSuccess == false)
        {
            return Result.Failure(result.Errors);
        }

        PasswordResetCode passwordResetCode = result.Value;

        await _repository.AddAsync(passwordResetCode, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        (string Email, string VerificationCode) model = (request.Email, verificationCode);
        string subject = "Reset Password";
        string senderEmail = "noreply@yourapp.com";
        string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/PasswordResetCodeTemplate", model);
        EmailMessage emailObject = new(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);
        await _emailSender.SendAsync(emailObject);

        return Result.Success();
    }
}
