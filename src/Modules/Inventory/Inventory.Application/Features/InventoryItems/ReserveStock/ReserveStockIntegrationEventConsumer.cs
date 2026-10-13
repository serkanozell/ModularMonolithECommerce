using Inventory.Contracts;
using MassTransit;

namespace Inventory.Application.Features.InventoryItems.ReserveStock
{
    public sealed class ReserveStockIntegrationEventConsumer(ISender sender) : IConsumer<ReserveStockIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ReserveStockIntegrationEvent> context)
        {
            var result = await sender.Send(
                new ReserveStockCommand(context.Message.OrderItems, context.Message.OrderId),
                context.CancellationToken);

            if (result.IsSuccess)
            {
                await context.Publish(
                    new StockReservedIntegrationEvent(
                        context.Message.SagaCorrelationId,
                        context.Message.OrderId),
                    context.CancellationToken);
            }
            else
            {
                await context.Publish(
                    new StockReserveFailedIntegrationEvent(
                        context.Message.SagaCorrelationId,
                        context.Message.OrderId,
                        result.FailureReason ?? "Stock reservation failed."),
                    context.CancellationToken);
            }
        }
    }
}