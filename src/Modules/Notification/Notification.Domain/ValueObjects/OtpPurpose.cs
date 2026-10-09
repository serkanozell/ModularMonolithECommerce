namespace Notification.Domain.ValueObjects
{
    public record OtpPurpose
    {
        public string Value { get; }

        private OtpPurpose(string value) => Value = value;

        public static OtpPurpose Of(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (value.Length > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "OtpPurpose cannot exceed 64 characters.");
            }

            return new OtpPurpose(value);
        }
    }
}