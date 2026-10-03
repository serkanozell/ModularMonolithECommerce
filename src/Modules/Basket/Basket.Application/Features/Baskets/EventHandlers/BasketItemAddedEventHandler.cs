using Basket.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Basket.Application.Features.Baskets.EventHandlers
{
    internal sealed class BasketItemAddedEventHandler(ILogger<BasketItemAddedEventHandler> logger)
        : INotificationHandler<BasketItemAddedEvent>
    {
        public Task Handle(BasketItemAddedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - BasketId: {BasketId}, ProductId: {ProductId}, Quantity: {Quantity}",
                                  notification.GetType().Name,
                                  notification.BasketId,
                                  notification.ProductId,
                                  notification.Quantity);

            // İleride: outbox'a integration event yazılacak.

            return Task.CompletedTask;
        }
    }
}
