using Inventory.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.InventoryItems.EventHandlers
{
    public sealed class StockAvailabilityChangedEventHandler(ILogger<StockAvailabilityChangedEventHandler> logger) : INotificationHandler<StockAvailabilityChangedEvent>
    {
        public Task Handle(StockAvailabilityChangedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - InventoryItemId: {InventoryItemId}, ProductId: {ProductId}, IsInStock: {IsInStock}, AvailableQuantity: {AvailableQuantity}",
                                  notification.GetType().Name,
                                  notification.InventoryItemId,
                                  notification.ProductId,
                                  notification.IsInStock,
                                  notification.AvailableQuantity);

            return Task.CompletedTask;
        }
    }
}