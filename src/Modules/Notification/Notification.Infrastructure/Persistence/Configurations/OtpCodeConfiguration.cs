namespace Notification.Infrastructure.Persistence.Configurations
{
    public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
    {
        public void Configure(EntityTypeBuilder<OtpCode> builder)
        {
            builder.Property(x => x.Id)
                   .HasConversion(id => id.Value, value => OtpCodeId.Of(value));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Recipient)
                   .HasConversion(recipient => recipient.Value, value => Recipient.Of(value))
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(x => x.Purpose)
                   .HasConversion(purpose => purpose.Value, value => OtpPurpose.Of(value))
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(x => x.CodeHash)
                   .HasConversion(codeHash => codeHash.Value, value => OtpCodeHash.Of(value))
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(x => x.ExpiresAt)
                   .HasConversion(expiration => expiration.Value, value => OtpExpiration.Of(value))
                   .IsRequired();

            builder.OwnsOne(x => x.Attempts, attempts =>
            {
                attempts.Property(value => value.Count)
                        .HasColumnName("attempt_count")
                        .IsRequired();

                attempts.Property(value => value.MaxAttempts)
                        .HasColumnName("max_attempts")
                        .IsRequired();
            });

            builder.Navigation(x => x.Attempts)
                   .IsRequired();

            builder.Property(x => x.Channel).HasConversion<string>().HasMaxLength(16).IsRequired();

            builder.Ignore(x => x.IsVerified);

            builder.HasIndex(x => new { x.Recipient, x.Purpose, x.IsActive });
            builder.HasIndex(x => new { x.Recipient, x.CreatedAt });
        }
    }
}
