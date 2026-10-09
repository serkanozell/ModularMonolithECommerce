namespace Inventory.Domain.ValueObjects
{
    public record OrderId
    {
        public Guid Value { get; }

        private OrderId(Guid value) => Value = value;

        public static OrderId Of(Guid value)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty);

            return new OrderId(value);
        }
    }
}