using Inventory.Contracts;
using MassTransit;

namespace Inventory.Application.Features.InventoryItems.ReleaseReserve
{
    public sealed class ReleaseReserveIntegrationEventConsumer(ISender sender) : IConsumer<ReleaseReserveIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ReleaseReserveIntegrationEvent> context)
        {
            await sender.Send(new ReleaseReserveCommand(context.Message.OrderId), context.CancellationToken);
        }
    }
}