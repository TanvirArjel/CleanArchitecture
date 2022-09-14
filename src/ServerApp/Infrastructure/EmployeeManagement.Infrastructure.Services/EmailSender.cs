using System;
using System.Threading.Tasks;
using EmployeeManagement.Application.Infrastrucures;
using EmployeeManagement.Infrastructure.Services.Configs;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmployeeManagement.Infrastructure.Services;

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

    public async Task SendAsync(EmailMessage emailMessage)
    {
        try
        {
            if (emailMessage == null)
            {
                throw new ArgumentNullException(nameof(emailMessage));
            }

            SendGridMessage message = new SendGridMessage()
            {
                Subject = emailMessage.Subject,
                HtmlContent = emailMessage.MailBody,
            };

            message.AddTo(new EmailAddress(emailMessage.ReceiverEmail, emailMessage.ReceiverName));

            if (!string.IsNullOrWhiteSpace(emailMessage.SenderEmail))
            {
                message.From = new EmailAddress(emailMessage.SenderEmail, emailMessage.SenderName);
                message.ReplyTo = new EmailAddress(emailMessage.SenderEmail, emailMessage.SenderName);
            }

            Response response = await SendGridClient.SendEmailAsync(message);
        }
        catch (Exception exception)
        {
            await _exceptionLogger.LogAsync(exception, emailMessage);
        }
    }
}
