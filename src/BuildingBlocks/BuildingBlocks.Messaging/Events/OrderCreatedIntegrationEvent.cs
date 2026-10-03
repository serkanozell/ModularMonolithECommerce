namespace BuildingBlocks.Messaging.Events
{
    public record OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string OrderName { get; set; } = default!;
        public string CustomerFirstName { get; set; } = default!;
        public string CustomerLastName { get; set; } = default!;
        public string? CustomerEmail { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
