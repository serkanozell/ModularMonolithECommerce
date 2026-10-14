namespace BuildingBlocks.Messaging.Events
{
    public record OrderCompletedIntegrationEvent(Guid OrderId) : IntegrationEvent;
}