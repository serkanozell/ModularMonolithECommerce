namespace Ordering.Domain.ValueObjects
{
    public record OrderName
    {
        public string Value { get; }

        private OrderName(string value) => Value = value;

        public static OrderName Of(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (value.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "OrderName cannot exceed 100 characters.");
            }

            return new OrderName(value);
        }
    }
}