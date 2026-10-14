namespace BuildingBlocks.Messaging.Events
{
    public record ConfirmOrderIntegrationEvent(Guid CorrelationId, Guid OrderId) : IntegrationEvent;
}