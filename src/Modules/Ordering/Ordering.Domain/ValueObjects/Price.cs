namespace Ordering.Domain.ValueObjects
{
    public record Price
    {
        public decimal Value { get; }

        private Price(decimal value) => Value = value;

        public static Price Of(decimal value)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            return new Price(value);
        }
    }
}