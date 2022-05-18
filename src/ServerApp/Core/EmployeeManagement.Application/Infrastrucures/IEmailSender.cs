using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Infrastrucures;

[SingletonService]
public interface IEmailSender
{
    Task SendAsync(EmailObject emailObject);
}

public class EmailObject
{
    public EmailObject(string receiverEmail, string subject, string mailBody)
    {
        ReceiverEmail = receiverEmail.ThrowIfNullOrEmpty(nameof(receiverEmail));
        Subject = subject.ThrowIfNullOrEmpty(nameof(subject));
        MailBody = mailBody.ThrowIfNullOrEmpty(nameof(mailBody));
    }

    public EmailObject(string receiverEmail, string receiverName, string subject, string mailBody)
    {
        ReceiverEmail = receiverEmail.ThrowIfNullOrEmpty(nameof(receiverEmail));
        ReceiverName = receiverName.ThrowIfNullOrEmpty(nameof(receiverName));
        Subject = subject.ThrowIfNullOrEmpty(nameof(subject));
        MailBody = mailBody.ThrowIfNullOrEmpty(nameof(mailBody));
    }

    public EmailObject(
        string receiverEmail,
        string receiverName,
        string senderEmail,
        string senderName,
        string subject,
        string mailBody)
    {
        ReceiverEmail = receiverEmail.ThrowIfNullOrEmpty(nameof(receiverEmail));
        ReceiverName = receiverName.ThrowIfNullOrEmpty(nameof(receiverName));
        SenderEmail = senderEmail.ThrowIfNullOrEmpty(nameof(senderEmail));
        SenderName = senderName.ThrowIfNullOrEmpty(nameof(senderName));
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
