using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Ordering.Application.Features.Orders.CancelOrder
{
    public class CancelOrderIntegrationEventConsumer(ISender sender) : IConsumer<CancelOrderIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<CancelOrderIntegrationEvent> context)
        {
            await sender.Send(new CancelOrderCommand(context.Message.OrderId, context.Message.Reason), context.CancellationToken);
        }
    }
}