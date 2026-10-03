namespace Notification.Application.Abstractions
{
    public record EmailMessage(string To, string Subject, string HtmlBody);

    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body, bool isHtml, CancellationToken cancellationToken = default);
    }
}