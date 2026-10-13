namespace BuildingBlocks.Messaging.Events
{
    public record CancelOrderIntegrationEvent(Guid CorrelationId, Guid OrderId, string Reason) : IntegrationEvent;
}