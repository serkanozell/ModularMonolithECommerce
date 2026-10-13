namespace BuildingBlocks.Messaging.Events
{
    public record OrderCanceledIntegrationEvent(Guid OrderId, string Reason) : IntegrationEvent;
}