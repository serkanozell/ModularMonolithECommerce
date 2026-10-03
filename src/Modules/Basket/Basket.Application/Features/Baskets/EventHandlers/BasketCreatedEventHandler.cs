using Basket.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Basket.Application.Features.Baskets.EventHandlers
{
    internal sealed class BasketCreatedEventHandler(ILogger<BasketCreatedEventHandler> logger)
        : INotificationHandler<BasketCreatedEvent>
    {
        public Task Handle(BasketCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent} - BasketId: {BasketId}, UserName: {UserName}",
                                  notification.GetType().Name,
                                  notification.BasketId,
                                  notification.UserName);

            // İleride: outbox'a integration event yazılacak.

            return Task.CompletedTask;
        }
    }
}
