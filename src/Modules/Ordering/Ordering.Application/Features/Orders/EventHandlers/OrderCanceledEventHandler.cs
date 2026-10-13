using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.Application.Features.Orders.EventHandlers
{
    public sealed class OrderCanceledEventHandler(IPublishEndpoint publishEndpoint, ILogger<OrderCanceledEventHandler> logger) : INotificationHandler<OrderCanceledEvent>
    {
        public Task Handle(OrderCanceledEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - OrderId: {OrderId}, Reason: {Reason}",
                                  notification.GetType().Name,
                                  notification.OrderId,
                                  notification.Reason);

            var orderCanceledIntegrationEvent = new OrderCanceledIntegrationEvent(notification.OrderId, notification.Reason);

            return publishEndpoint.Publish(orderCanceledIntegrationEvent, cancellationToken);
        }
    }
}