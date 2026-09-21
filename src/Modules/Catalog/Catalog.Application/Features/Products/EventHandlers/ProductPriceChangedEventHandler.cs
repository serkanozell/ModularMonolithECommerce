using Catalog.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Products.EventHandlers
{
    public class ProductPriceChangedEventHandler(ILogger<ProductPriceChangedEventHandler> logger)
        : INotificationHandler<ProductPriceChangedEvent>
    {
        public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - ProductId: {ProductId}, {OldPrice} -> {NewPrice}",
                                  notification.GetType().Name,
                                  notification.ProductId,
                                  notification.OldPrice,
                                  notification.NewPrice);

            // İleride: outbox'a integration event yazılacak.

            return Task.CompletedTask;
        }
    }
}