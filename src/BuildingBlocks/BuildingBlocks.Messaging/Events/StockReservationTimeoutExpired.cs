namespace BuildingBlocks.Messaging.Events
{
    public sealed record StockReservationTimeoutExpired(Guid OrderId);
}