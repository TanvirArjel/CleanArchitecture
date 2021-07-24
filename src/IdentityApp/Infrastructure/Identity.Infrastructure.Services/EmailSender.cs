using System;
using System.Threading.Tasks;
using Identity.Application.Infrastrucures;
using Identity.Infrastructure.Services.Configs;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Identity.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridConfig _sendGridConfig;
        private readonly IExceptionLogger _exceptionLogger;

        public EmailSender(SendGridConfig sendGridConfig, IExceptionLogger exceptionLogger)
        {
            _exceptionLogger = exceptionLogger;
            _sendGridConfig = sendGridConfig;
        }

        private SendGridClient SendGridClient => new SendGridClient(_sendGridConfig.ApiKey);

        public async Task SendAsync(EmailObject emailObject)
        {
            try
            {
                if (emailObject == null)
                {
                    throw new ArgumentNullException(nameof(emailObject));
                }

                SendGridMessage message = new SendGridMessage()
                {
                    Subject = emailObject.Subject,
                    HtmlContent = emailObject.MailBody,
                };

                message.AddTo(new EmailAddress(emailObject.ReceiverEmail, emailObject.ReceiverName));

                if (!string.IsNullOrWhiteSpace(emailObject.SenderEmail))
                {
                    message.From = new EmailAddress(emailObject.SenderEmail, emailObject.SenderName);
                    message.ReplyTo = new EmailAddress(emailObject.SenderEmail, emailObject.SenderName);
                }

                Response response = await SendGridClient.SendEmailAsync(message);
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception, emailObject);
            }
        }
    }
}
