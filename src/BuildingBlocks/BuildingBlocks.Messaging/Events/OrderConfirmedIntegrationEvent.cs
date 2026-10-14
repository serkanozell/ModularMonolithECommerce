namespace BuildingBlocks.Messaging.Events
{
    public record OrderConfirmedIntegrationEvent(Guid OrderId) : IntegrationEvent;
}