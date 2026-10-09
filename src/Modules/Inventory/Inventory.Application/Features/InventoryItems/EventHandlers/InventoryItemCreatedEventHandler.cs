using Inventory.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.InventoryItems.EventHandlers
{
    public sealed class InventoryItemCreatedEventHandler(ILogger<InventoryItemCreatedEventHandler> logger) : INotificationHandler<InventoryItemCreatedEvent>
    {
        public Task Handle(InventoryItemCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - InventoryItemId: {InventoryItemId}, ProductId: {ProductId}, InitialQuantity: {InitialQuantity}",
                                  notification.GetType().Name,
                                  notification.InventoryItemId.Value,
                                  notification.ProductId.Value,
                                  notification.InitialQuantity);

            return Task.CompletedTask;
        }
    }
}