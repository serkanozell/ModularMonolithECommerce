namespace BuildingBlocks.Messaging.Events
{
    public interface IIntegrationEvent
    {
        Guid EventId { get; }
        DateTime OccurredOnUtc { get; }
        string EventType { get; }
    }

    public abstract record IntegrationEvent : IIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
        public string EventType => GetType().AssemblyQualifiedName!;
    }
}