using Basket.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Basket.Application.Features.Baskets.EventHandlers
{
    internal sealed class BasketItemRemovedEventHandler(ILogger<BasketItemRemovedEventHandler> logger)
        : INotificationHandler<BasketItemRemovedEvent>
    {
        public Task Handle(BasketItemRemovedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - BasketId: {BasketId}, ProductId: {ProductId}",
                                  notification.GetType().Name,
                                  notification.BasketId,
                                  notification.ProductId);

            // İleride: outbox'a integration event yazılacak.

            return Task.CompletedTask;
        }
    }
}
