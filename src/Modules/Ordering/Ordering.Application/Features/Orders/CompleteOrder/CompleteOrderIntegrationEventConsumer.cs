using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Ordering.Application.Features.Orders.CompleteOrder
{
    public class CompleteOrderIntegrationEventConsumer(ISender sender) : IConsumer<CompleteOrderIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<CompleteOrderIntegrationEvent> context)
        {
            await sender.Send(new CompleteOrderCommand(context.Message.OrderId), context.CancellationToken);
        }
    }
}