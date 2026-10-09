namespace Notification.Domain.Entities
{
    public class OtpCode : Aggregate<OtpCodeId>
    {
        public Recipient Recipient { get; private set; } = default!;
        public NotificationChannel Channel { get; private set; }
        public OtpPurpose Purpose { get; private set; } = default!; // kullanım amacı, login, password reset, email verification vb.
        public OtpCodeHash CodeHash { get; private set; } = default!;
        public OtpExpiration ExpiresAt { get; private set; } = default!;
        public OtpAttempts Attempts { get; private set; } = default!;
        public DateTime? VerifiedAt { get; private set; }

        public bool IsVerified => VerifiedAt.HasValue;

        private OtpCode() { }

        private OtpCode(Recipient recipient, NotificationChannel channel, OtpPurpose purpose, OtpCodeHash codeHash, OtpExpiration expiresAt, OtpAttempts attempts)
        {
            Id = OtpCodeId.Of(Guid.NewGuid());
            Recipient = recipient;
            Channel = channel;
            Purpose = purpose;
            CodeHash = codeHash;
            ExpiresAt = expiresAt;
            Attempts = attempts;
            IsActive = true;
            IsDeleted = false;
        }

        public static OtpCode Create(Recipient recipient, NotificationChannel channel, OtpPurpose purpose, string codeHash, TimeSpan lifetime, int maxAttempts)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(purpose);
            ArgumentException.ThrowIfNullOrWhiteSpace(codeHash);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lifetime, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);

            return new OtpCode(recipient,
                               channel,
                               purpose,
                               OtpCodeHash.Of(codeHash),
                               OtpExpiration.FromLifetime(lifetime),
                               OtpAttempts.Create(maxAttempts));
        }

        public bool CanBeVerified() => IsActive
                                       && !IsDeleted
                                       && !IsVerified
                                       && Attempts.HasAttemptsRemaining
                                       && !ExpiresAt.HasExpired(DateTime.UtcNow);

        public bool Verify(string codeHash)
        {
            if (!CanBeVerified())
                return false;

            Attempts = Attempts.RecordAttempt();

            if (!string.Equals(CodeHash.Value, codeHash, StringComparison.Ordinal))
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