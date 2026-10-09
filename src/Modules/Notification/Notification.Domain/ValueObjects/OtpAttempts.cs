namespace Notification.Domain.ValueObjects
{
    public record OtpAttempts
    {
        public int Count { get; }
        public int MaxAttempts { get; }
        public bool HasAttemptsRemaining => Count < MaxAttempts;

        public OtpAttempts(int count, int maxAttempts)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count, maxAttempts);

            Count = count;
            MaxAttempts = maxAttempts;
        }

        public static OtpAttempts Create(int maxAttempts) => new(0, maxAttempts);

        public OtpAttempts RecordAttempt()
        {
            if (!HasAttemptsRemaining)
                throw new InvalidOperationException("The maximum number of OTP attempts has been reached.");

            return new OtpAttempts(checked(Count + 1), MaxAttempts);
        }
    }
}