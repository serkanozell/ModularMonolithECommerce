using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;

namespace Notification.Application.Features.Orders.EventHandlers
{
    public sealed class OrderCreatedIntegrationEventHandler(IEmailSender emailSender, ILogger<OrderCreatedIntegrationEventHandler> logger) : IConsumer<OrderCreatedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var message = context.Message;

            logger.LogInformation("OrderCreatedIntegrationEventHandler received a message: OrderId: {OrderId}", message.OrderId);

            if (string.IsNullOrWhiteSpace(message.CustomerEmail))
            {
                logger.LogWarning("Order {OrderId} has no customer email, confirmation mail skipped.", message.OrderId);
                return;
            }

            var fullName = WebUtility.HtmlEncode($"{message.CustomerFirstName} {message.CustomerLastName}");
            var orderName = WebUtility.HtmlEncode(message.OrderName);
            var totalPrice = message.TotalPrice.ToString("N2", CultureInfo.GetCultureInfo("tr-TR"));

            var body = $"""
                <p>Merhaba {fullName},</p>
                <p><strong>{orderName}</strong> numaralı siparişiniz alınmıştır.</p>
                <p>Toplam tutar: {totalPrice} TL</p>
                <p>Bizi tercih ettiğiniz için teşekkür ederiz.</p>
                """;

            await emailSender.SendAsync(message.CustomerEmail, "Siparişiniz alındı", body, isHtml: true, context.CancellationToken);
        }
    }
}
