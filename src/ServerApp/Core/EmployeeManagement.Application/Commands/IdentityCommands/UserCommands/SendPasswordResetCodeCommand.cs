using System.Globalization;
using System.Security.Cryptography;
using EmployeeManagement.Application.Infrastrucures;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Commands.IdentityCommands.UserCommands;

public class SendPasswordResetCodeCommand : IRequest
{
    public SendPasswordResetCodeCommand(string email)
    {
        Email = email.ThrowIfNotValidEmail(nameof(email));
    }

    public string Email { get; }

    private class SendPasswordResetCodeCommandHandler : IRequestHandler<SendPasswordResetCodeCommand>
    {
        private readonly IRepository _repository;
        private readonly ViewRenderService _viewRenderService;
        private readonly IEmailSender _emailSender;

        public SendPasswordResetCodeCommandHandler(
            IRepository repository,
            ViewRenderService viewRenderService,
            IEmailSender emailSender)
        {
            _repository = repository;
            _viewRenderService = viewRenderService;
            _emailSender = emailSender;
        }

        public async Task<Unit> Handle(SendPasswordResetCodeCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExistent = await _repository.ExistsAsync<ApplicationUser>(u => u.Email == request.Email, cancellationToken);

            if (isExistent == false)
            {
                throw new InvalidOperationException("The user does not exist with the provided email.");
            }

            int randomNumber = RandomNumberGenerator.GetInt32(0, 1000000);
            string verificationCode = randomNumber.ToString("D6", CultureInfo.InvariantCulture);

            PasswordResetCode emailVerificationCode = new PasswordResetCode()
            {
                Code = verificationCode,
                Email = request.Email,
                SentAtUtc = DateTime.UtcNow
            };

            await _repository.InsertAsync(emailVerificationCode, cancellationToken);

            (string Email, string VerificationCode) model = (request.Email, verificationCode);
            string subject = "Reset Password";
            string senderEmail = "noreply@yourapp.com";
            string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/PasswordResetCodeTemplate", model);
            EmailObject emailObject = new EmailObject(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);
            await _emailSender.SendAsync(emailObject);

            return Unit.Value;
        }
    }
}
