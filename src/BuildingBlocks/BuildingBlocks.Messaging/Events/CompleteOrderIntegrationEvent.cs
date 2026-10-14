namespace BuildingBlocks.Messaging.Events
{
    public record CompleteOrderIntegrationEvent(Guid CorrelationId, Guid OrderId) : IntegrationEvent;
}