using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Persistance.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.Property(x => x.Id).HasConversion(id => id.Value, value => PaymentTransactionId.Of(value));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId).HasConversion(id => id.Value, value => OrderId.Of(value)).IsRequired();
            builder.HasIndex(x => x.OrderId).IsUnique();   // sipariş başına tek ödeme kaydı

            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.FailureReason).HasMaxLength(500);

            builder.Ignore(x => x.DomainEvents);
        }
    }
}