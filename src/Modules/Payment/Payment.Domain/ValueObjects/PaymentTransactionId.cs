namespace Payment.Domain.ValueObjects
{
    public record PaymentTransactionId
    {
        public Guid Value { get; }
        private PaymentTransactionId(Guid value) => Value = value;
        public static PaymentTransactionId Of(Guid value)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty);
            return new PaymentTransactionId(value);
        }
    }
}