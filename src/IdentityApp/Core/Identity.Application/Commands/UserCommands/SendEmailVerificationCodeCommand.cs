using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Infrastrucures;
using Identity.Application.Services;
using Identity.Domain.Entities;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace Identity.Application.Commands.UserCommands
{
    public class SendEmailVerificationCodeCommand : IRequest
    {
        public SendEmailVerificationCodeCommand(string email)
        {
            Email = email.ThrowIfNotValidEmail(nameof(email));
        }

        public string Email { get; }

        private class SendEmailVerificationCodeCommandHanlder : IRequestHandler<SendEmailVerificationCodeCommand>
        {
            private readonly IRepository _repository;
            private readonly IViewRenderService _viewRenderService;
            private readonly IEmailSender _emailSender;

            public SendEmailVerificationCodeCommandHanlder(
                IRepository repository,
                IViewRenderService viewRenderService,
                IEmailSender emailSender)
            {
                _repository = repository;
                _viewRenderService = viewRenderService;
                _emailSender = emailSender;
            }

            public async Task<Unit> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
            {
                request.ThrowIfNull(nameof(request));

                Random generator = new Random();
                string verificationCode = generator.Next(0, 1000000).ToString("D6", CultureInfo.InvariantCulture);

                EmailVerificationCode emailVerificationCode = new EmailVerificationCode()
                {
                    Code = verificationCode,
                    Email = request.Email,
                    SentAtUtc = DateTime.UtcNow
                };

                await _repository.InsertAsync(emailVerificationCode);

                (string Email, string VerificationCode) model = (request.Email, verificationCode);
                string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/ConfirmRegistrationCodeTemplate", model);

                string senderEmail = "noreply@yourapp.com";
                string subject = "User Registration";

                EmailObject emailObject = new EmailObject(request.Email, request.Email, senderEmail, senderEmail, subject, emailBody);

                await _emailSender.SendAsync(emailObject);

                return Unit.Value;
            }
        }
    }
}
