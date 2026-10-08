using BuildingBlocks.Messaging.Events;
using Inventory.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.InventoryItems.EventHandlers
{
    public sealed class StockAvailabilityChangedEventHandler(IPublishEndpoint publishEndpoint, ILogger<StockAvailabilityChangedEventHandler> logger) : INotificationHandler<StockAvailabilityChangedEvent>
    {
        public async Task Handle(StockAvailabilityChangedEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new StockAvailabilityChangedIntegrationEvent
            {
                InventoryItemId = notification.InventoryItemId,
                ProductId = notification.ProductId,
                IsInStock = notification.IsInStock,
                AvailableQuantity = notification.AvailableQuantity
            };

            logger.LogInformation("Domain event handled: {DomainEvent} - InventoryItemId: {InventoryItemId}, ProductId: {ProductId}, IsInStock: {IsInStock}, AvailableQuantity: {AvailableQuantity}",
                                  notification.GetType().Name,
                                  notification.InventoryItemId,
                                  notification.ProductId,
                                  notification.IsInStock,
                                  notification.AvailableQuantity);

            await publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
    }
}