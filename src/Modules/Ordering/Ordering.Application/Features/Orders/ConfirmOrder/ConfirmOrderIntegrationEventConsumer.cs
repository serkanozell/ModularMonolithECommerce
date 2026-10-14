using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Ordering.Application.Features.Orders.ConfirmOrder
{
    public sealed class ConfirmOrderIntegrationEventConsumer(ISender sender) : IConsumer<ConfirmOrderIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ConfirmOrderIntegrationEvent> context)
        {
            await sender.Send(new ConfirmOrderCommand(context.Message.OrderId), context.CancellationToken);
        }
    }
}