namespace Notification.Infrastructure.Persistence.Configurations
{
    public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
    {
        public void Configure(EntityTypeBuilder<OtpCode> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Recipient).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Purpose).HasMaxLength(64).IsRequired();
            builder.Property(x => x.CodeHash).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Channel).HasConversion<string>().HasMaxLength(16).IsRequired();

            builder.Ignore(x => x.IsVerified);

            builder.HasIndex(x => new { x.Recipient, x.Purpose, x.IsActive });
            builder.HasIndex(x => new { x.Recipient, x.CreatedAt });
        }
    }
}
