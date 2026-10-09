namespace Ordering.Domain.ValueObjects
{
    public record OrderItemId
    {
        public Guid Value { get; }

        private OrderItemId(Guid value) => Value = value;

        public static OrderItemId Of(Guid value)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty);

            return new OrderItemId(value);
        }
    }
}