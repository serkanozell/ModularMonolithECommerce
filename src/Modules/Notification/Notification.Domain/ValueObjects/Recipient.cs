namespace Notification.Domain.ValueObjects
{
    public record Recipient
    {
        public string Value { get; }

        private Recipient(string value) => Value = value;

        public static Recipient Of(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (value.Length > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Recipient cannot exceed 256 characters.");
            }

            return new Recipient(value);
        }
    }
}