namespace Notification.Domain.ValueObjects
{
    public record OtpCodeHash
    {
        public string Value { get; }

        private OtpCodeHash(string value) => Value = value;

        public static OtpCodeHash Of(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (value.Length > 128)
                throw new ArgumentOutOfRangeException(nameof(value), "OTP code hash cannot exceed 128 characters.");

            return new OtpCodeHash(value);
        }
    }
}