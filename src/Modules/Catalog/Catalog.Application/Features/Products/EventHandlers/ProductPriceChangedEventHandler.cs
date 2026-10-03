using BuildingBlocks.Messaging.Events;
using Catalog.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Products.EventHandlers
{
    public sealed class ProductPriceChangedEventHandler(IPublishEndpoint publishEndpoint, ILogger<ProductPriceChangedEventHandler> logger) : INotificationHandler<ProductPriceChangedEvent>
    {
        public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - ProductId: {ProductId}, {OldPrice} -> {NewPrice}",
                                  notification.GetType().Name,
                                  notification.ProductId,
                                  notification.OldPrice,
                                  notification.NewPrice);


            var productPriceChangedIntegrationEvent = new ProductPriceChangedIntegrationEvent
            {
                ProductId = notification.ProductId,
                OldPrice = notification.OldPrice,
                NewPrice = notification.NewPrice
            };

            await publishEndpoint.Publish(productPriceChangedIntegrationEvent, cancellationToken);
        }
    }
}