using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Application.Features.Otp.EventHandlers
{
    internal sealed class SendOtpIntegrationEventHandler(IEmailSender emailSender, ILogger<SendOtpIntegrationEventHandler> logger) : IConsumer<SendOtpIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<SendOtpIntegrationEvent> context)
        {
            logger.LogInformation("Received SendOtpIntegrationEvent: {Event}", context.Message);

            await emailSender.SendAsync(
                context.Message.Recipient,
                context.Message.Purpose,
                context.Message.Text,
                true
            );

            logger.LogInformation("Email sent to {Recipient} for purpose {Purpose}", context.Message.Recipient, context.Message.Purpose);
        }
    }
}