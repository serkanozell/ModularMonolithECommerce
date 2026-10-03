namespace Notification.Application.Abstractions
{
    public record SmsMessage(string PhoneNumber, string Text);

    public interface ISmsSender
    {
        Task SendAsync(SmsMessage message, CancellationToken cancellationToken = default);
    }
}
