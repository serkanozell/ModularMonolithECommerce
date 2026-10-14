using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.Application.Features.Orders.EventHandlers
{
    public sealed class OrderConfirmedEventHandler(IPublishEndpoint publishEndpoint, ILogger<OrderConfirmedEventHandler> logger) : INotificationHandler<OrderConfirmedEvent>
    {
        public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - OrderId: {OrderId}",
                                  notification.GetType().Name,
                                  notification.OrderId);

            await publishEndpoint.Publish(new OrderConfirmedIntegrationEvent(notification.OrderId), cancellationToken);
        }
    }
}