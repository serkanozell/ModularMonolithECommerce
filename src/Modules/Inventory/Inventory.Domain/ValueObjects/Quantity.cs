namespace Inventory.Domain.ValueObjects
{
    public record Quantity
    {
        public int Value { get; }

        private Quantity(int value) => Value = value;

        public static Quantity Of(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            return new Quantity(value);
        }
    }
}