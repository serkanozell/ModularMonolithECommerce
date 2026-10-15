namespace BuildingBlocks.Messaging.Events
{
    public sealed record SagaTimeoutExpired(Guid OrderId);
}