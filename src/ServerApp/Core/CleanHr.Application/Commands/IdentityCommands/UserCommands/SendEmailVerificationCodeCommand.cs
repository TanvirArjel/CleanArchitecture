using System.Globalization;
using System.Security.Cryptography;
using CleanHr.Application.Infrastructures;
using CleanHr.Application.Services;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class SendEmailVerificationCodeCommand(string email) : IRequest<Result>
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));
}

internal class SendEmailVerificationCodeCommandHandler : IRequestHandler<SendEmailVerificationCodeCommand, Result>
{
    private readonly IRepository _repository;
    private readonly ViewRenderService _viewRenderService;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<ApplicationUser> _userManager;

    public SendEmailVerificationCodeCommandHandler(
        IRepository repository,
        ViewRenderService viewRenderService,
        IEmailSender emailSender,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        // Validate that user exists
        ApplicationUser applicationUser = await _userManager.FindByEmailAsync(request.Email);

        if (applicationUser == null)
        {
            return Result.Failure("Email", "The provided email is not related to any account.");
        }

        if (applicationUser.EmailConfirmed)
        {
            return Result.Failure("Email", "The email is already confirmed.");
        }

        int randomNumber = RandomNumberGenerator.GetInt32(0, 1000000);
        string verificationCode = randomNumber.ToString("D6", CultureInfo.InvariantCulture);

        Result<EmailVerificationCode> result = await EmailVerificationCode.CreateAsync(_userManager, request.Email, verificationCode);

        if (result.IsSuccess == false)
        {
            return Result.Failure(result.Errors);
        }

        EmailVerificationCode emailVerificationCode = result.Value;

        await _repository.AddAsync(emailVerificationCode, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        (string Email, string VerificationCode) model = (request.Email, verificationCode);
        string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/ConfirmRegistrationCodeTemplate", model);

        string senderEmail = "noreply@yourapp.com";
        string subject = "User Registration";

        EmailMessage emailObject = new(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);

        await _emailSender.SendAsync(emailObject);

        return Result.Success();
    }
}
