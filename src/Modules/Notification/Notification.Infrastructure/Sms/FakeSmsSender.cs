using Microsoft.Extensions.Logging;
using Notification.Application.Abstractions;

namespace Notification.Infrastructure.Sms
{
    public sealed class FakeSmsSender(ILogger<FakeSmsSender> logger) : ISmsSender
    {
        public Task SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("[FAKE SMS] To: {PhoneNumber} - Text: {Text}", message.PhoneNumber, message.Text);

            return Task.CompletedTask;
        }
    }
}
