namespace Notification.Domain.ValueObjects
{
    public record OtpExpiration
    {
        public DateTime Value { get; }

        private OtpExpiration(DateTime value) => Value = value;

        public static OtpExpiration Of(DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
                throw new ArgumentException("OTP expiration must be UTC.", nameof(value));

            return new OtpExpiration(value);
        }

        public static OtpExpiration FromLifetime(TimeSpan lifetime)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lifetime, TimeSpan.Zero);

            return Of(DateTime.UtcNow.Add(lifetime));
        }

        public bool HasExpired(DateTime utcNow)
        {
            if (utcNow.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Current time must be UTC.", nameof(utcNow));

            return Value <= utcNow;
        }
    }
}