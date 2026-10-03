using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.Application.Features.Orders.EventHandlers
{
    public sealed class OrderCreatedEventHandler(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedEventHandler> logger) : INotificationHandler<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;

            logger.LogInformation("Domain event handled: {DomainEvent} - OrderId: {OrderId}, CustomerId: {CustomerId}, OrderName: {OrderName}",
                                  notification.GetType().Name,
                                  order.Id,
                                  order.CustomerId,
                                  order.OrderName);

            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderName = order.OrderName,
                CustomerFirstName = order.ShippingAddress.FirstName,
                CustomerLastName = order.ShippingAddress.LastName,
                CustomerEmail = order.ShippingAddress.EmailAddress,
                TotalPrice = order.TotalPrice
            };

            await publishEndpoint.Publish(orderCreatedIntegrationEvent, cancellationToken);
        }
    }
}