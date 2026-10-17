using Inventory.Contracts;
using MassTransit;

namespace Inventory.Application.Features.InventoryItems.ReleaseReserve
{
    public sealed class ReleaseReserveIntegrationEventConsumer(ISender sender) : IConsumer<ReleaseReserveIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ReleaseReserveIntegrationEvent> context)
        {
            var result = await sender.Send(new ReleaseReserveCommand(context.Message.OrderId), context.CancellationToken);

            await context.Publish(
                new StockReservationReleaseResultIntegrationEvent(
                    context.Message.SagaCorrelationId,
                    context.Message.OrderId,
                    result.IsSuccess,
                    result.IsSuccess ? null : "Stock reservation release failed."),
                context.CancellationToken);
        }
    }
}