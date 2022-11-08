using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Infrastructures;

[SingletonService]
public interface IEmailSender
{
    Task SendAsync(EmailMessage emailMessage);
}

public sealed class EmailMessage
{
    public EmailMessage(string receiverEmail, string subject, string mailBody)
        : this(receiverEmail, receiverName: null, subject, mailBody)
    {
    }

    public EmailMessage(string receiverEmail, string receiverName, string subject, string mailBody)
        : this(receiverEmail, receiverName, senderEmail: null, senderName: null, subject, mailBody)
    {
    }

    public EmailMessage(
        string receiverEmail,
        string receiverName,
        string senderEmail,
        string senderName,
        string subject,
        string mailBody)
    {
        ReceiverEmail = receiverEmail.ThrowIfNullOrEmpty(nameof(receiverEmail));
        ReceiverName = receiverName;
        SenderEmail = senderEmail != null ? senderEmail.ThrowIfNotValidEmail(nameof(senderEmail)) : senderEmail;
        SenderName = senderName;
        Subject = subject.ThrowIfNullOrEmpty(nameof(subject));
        MailBody = mailBody.ThrowIfNullOrEmpty(nameof(mailBody));
    }

    public string ReceiverEmail { get; private set; }

    public string ReceiverName { get; private set; }

    public string SenderEmail { get; private set; }

    public string SenderName { get; private set; }

    public string Subject { get; private set; }

    public string MailBody { get; private set; }
}
