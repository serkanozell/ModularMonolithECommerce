using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.Application.Features.Orders.EventHandlers
{
    public sealed class OrderCompletedEventHandler(IPublishEndpoint publishEndpoint, ILogger<OrderCompletedEventHandler> logger) : INotificationHandler<OrderCompletedEvent>
    {
        public async Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - OrderId: {OrderId}",
                                  notification.GetType().Name, notification.OrderId);

            await publishEndpoint.Publish(new OrderCompletedIntegrationEvent(notification.OrderId), cancellationToken);
        }
    }
}