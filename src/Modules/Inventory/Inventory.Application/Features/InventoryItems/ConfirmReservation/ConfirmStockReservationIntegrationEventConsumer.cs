using Inventory.Contracts;
using MassTransit;

namespace Inventory.Application.Features.InventoryItems.ConfirmReservation
{
    public sealed class ConfirmStockReservationIntegrationEventConsumer(ISender sender) : IConsumer<ConfirmStockReservationIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ConfirmStockReservationIntegrationEvent> context)
        {
            var result = await sender.Send(new ConfirmReservationCommand(context.Message.OrderId), context.CancellationToken);

            if (result.IsSuccess)
            {
                await context.Publish(
                    new StockReservationConfirmedIntegrationEvent(context.Message.SagaCorrelationId, context.Message.OrderId),
                    context.CancellationToken);
            }
            else
            {
                await context.Publish(
                    new StockReservationConfirmFailedIntegrationEvent(context.Message.SagaCorrelationId,
                                                                      context.Message.OrderId,
                                                                      result.FailureReason ?? "Stock reservation confirmation failed."),
                    context.CancellationToken);
            }
        }
    }
}
