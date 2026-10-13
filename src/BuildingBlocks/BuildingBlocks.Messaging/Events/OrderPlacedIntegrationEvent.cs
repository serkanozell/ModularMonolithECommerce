using BuildingBlocks.Shared.Dtos;

namespace BuildingBlocks.Messaging.Events
{
    public record OrderPlacedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> OrderItems { get; init; } = [];
        public decimal TotalPrice { get; set; }
    }
}