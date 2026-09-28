using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.Application.Features.Orders.EventHandlers
{
    public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) : INotificationHandler<OrderCreatedEvent>
    {
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - OrderId: {OrderId}, CustomerId: {CustomerId}, OrderName: {OrderName}",
                                  notification.GetType().Name,
                                  notification.OrderId,
                                  notification.CustomerId,
                                  notification.OrderName);

            return Task.CompletedTask;
        }
    }
}
