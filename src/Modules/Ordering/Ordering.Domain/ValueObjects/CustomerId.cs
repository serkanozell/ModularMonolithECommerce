namespace Ordering.Domain.ValueObjects
{
    public record CustomerId
    {
        public Guid Value { get; }

        private CustomerId(Guid value) => Value = value;

        public static CustomerId Of(Guid value)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty);

            return new CustomerId(value);
        }
    }
}