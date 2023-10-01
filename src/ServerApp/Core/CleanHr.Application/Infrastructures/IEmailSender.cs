using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Infrastructures;

[SingletonService]
public interface IEmailSender
{
    Task SendAsync(EmailMessage emailMessage);
}

public sealed class EmailMessage(
    string receiverEmail,
    string receiverName,
    string senderEmail,
    string senderName,
    string subject,
    string mailBody)
{
    public EmailMessage(string receiverEmail, string subject, string mailBody)
        : this(receiverEmail, receiverName: null, subject, mailBody)
    {
    }

    public EmailMessage(string receiverEmail, string receiverName, string subject, string mailBody)
        : this(receiverEmail, receiverName, senderEmail: null, senderName: null, subject, mailBody)
    {
    }

    public string ReceiverEmail { get; } = receiverEmail.ThrowIfNullOrEmpty(nameof(receiverEmail));

    public string ReceiverName { get; } = receiverName;

    public string SenderEmail { get; } = senderEmail != null ? senderEmail.ThrowIfNotValidEmail(nameof(senderEmail)) : senderEmail;

    public string SenderName { get; } = senderName;

    public string Subject { get; } = subject.ThrowIfNullOrEmpty(nameof(subject));

    public string MailBody { get; } = mailBody.ThrowIfNullOrEmpty(nameof(mailBody));
}
