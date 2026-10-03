namespace Notification.Domain.Entities
{
    public class OtpCode : Aggregate<Guid>
    {
        public string Recipient { get; private set; } = default!;
        public NotificationChannel Channel { get; private set; }
        public string Purpose { get; private set; } = default!; // kullanım amacı, login, password reset, email verification vb.
        public string CodeHash { get; private set; } = default!;
        public DateTime ExpiresAt { get; private set; }
        public int AttemptCount { get; private set; }
        public int MaxAttempts { get; private set; }
        public DateTime? VerifiedAt { get; private set; }

        public bool IsVerified => VerifiedAt.HasValue;

        private OtpCode() { }

        private OtpCode(string recipient, NotificationChannel channel, string purpose, string codeHash, DateTime expiresAt, int maxAttempts)
        {
            Id = Guid.NewGuid();
            Recipient = recipient;
            Channel = channel;
            Purpose = purpose;
            CodeHash = codeHash;
            ExpiresAt = expiresAt;
            MaxAttempts = maxAttempts;
            AttemptCount = 0;
            IsActive = true;
            IsDeleted = false;
        }

        public static OtpCode Create(string recipient, NotificationChannel channel, string purpose, string codeHash, TimeSpan lifetime, int maxAttempts)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
            ArgumentException.ThrowIfNullOrWhiteSpace(purpose);
            ArgumentException.ThrowIfNullOrWhiteSpace(codeHash);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lifetime, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);

            return new OtpCode(recipient, channel, purpose, codeHash, DateTime.UtcNow.Add(lifetime), maxAttempts);
        }

        public bool CanBeVerified() =>
            IsActive && !IsVerified && AttemptCount < MaxAttempts && ExpiresAt > DateTime.UtcNow;

        public bool Verify(string codeHash)
        {
            if (!CanBeVerified())
                return false;

            AttemptCount++;

            if (!string.Equals(CodeHash, codeHash, StringComparison.Ordinal))
                return false;

            VerifiedAt = DateTime.UtcNow;
            IsActive = false;
            IsDeleted = true;
            return true;
        }

        public void Invalidate()
        {
            IsActive = false;
            IsDeleted = true;
        }
    }
}