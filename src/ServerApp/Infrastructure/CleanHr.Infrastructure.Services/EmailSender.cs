using System;
using System.Threading.Tasks;
using CleanHr.Application.Infrastructures;
using CleanHr.Infrastructure.Services.Configs;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CleanHr.Infrastructure.Services;

public sealed class EmailSender(SendGridConfig sendGridConfig, IExceptionLogger exceptionLogger) : IEmailSender
{
    private readonly SendGridConfig _sendGridConfig = sendGridConfig ?? throw new ArgumentNullException(nameof(sendGridConfig));
    private readonly IExceptionLogger _exceptionLogger = exceptionLogger ?? throw new ArgumentNullException(nameof(exceptionLogger));

    private SendGridClient SendGridClient => new(_sendGridConfig.ApiKey);

    public async Task SendAsync(EmailMessage emailMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(emailMessage);

            SendGridMessage message = new()
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
