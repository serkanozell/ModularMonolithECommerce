using Catalog.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Products.EventHandlers
{
    public sealed class ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger) : INotificationHandler<ProductCreatedEvent>
    {
        public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - ProductId: {ProductId}, Name: {Name}",
                                  notification.GetType().Name,
                                  notification.ProductId.Value,
                                  notification.Name.Value);

            // İleride: outbox'a integration event yazılacak.

            return Task.CompletedTask;
        }
    }
}