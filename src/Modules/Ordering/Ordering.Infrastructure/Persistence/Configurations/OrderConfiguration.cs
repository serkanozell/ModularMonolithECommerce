namespace Ordering.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.Property(x => x.Id)
                   .HasConversion(id => id.Value, value => OrderId.Of(value));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId)
                   .HasConversion(id => id.Value, value => CustomerId.Of(value))
                   .IsRequired();

            builder.HasIndex(x => x.CustomerId);

            builder.Property(x => x.OrderName)
                   .HasConversion(name => name.Value, value => OrderName.Of(value))
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.OrderStatus)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.ComplexProperty(x => x.ShippingAddress, ConfigureAddress);

            builder.ComplexProperty(x => x.BillingAddress, ConfigureAddress);

            builder.ComplexProperty(x => x.Payment, payment =>
            {
                payment.Property(p => p.CardName).HasMaxLength(50);
                payment.Property(p => p.CardNumber).IsRequired().HasMaxLength(24);
                payment.Property(p => p.Expiration).IsRequired().HasMaxLength(10);
                payment.Property(p => p.CVV).IsRequired().HasMaxLength(3);
                payment.Property(p => p.PaymentMethod).IsRequired();
            });

            builder.HasMany(x => x.Items)
                   .WithOne()
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.Metadata
            //       .FindNavigation(nameof(Order.Items))!
            //       .SetPropertyAccessMode(PropertyAccessMode.Field);

            //builder.Ignore(x => x.TotalPrice);

            //builder.Ignore(x => x.DomainEvents);

            //builder.HasQueryFilter(x => !x.IsDeleted);
        }

        private static void ConfigureAddress(ComplexPropertyBuilder<Domain.ValueObjects.Address> address)
        {
            address.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
            address.Property(a => a.LastName).IsRequired().HasMaxLength(50);
            address.Property(a => a.EmailAddress).HasMaxLength(100);
            address.Property(a => a.AddressLine).IsRequired().HasMaxLength(200);
            address.Property(a => a.Country).HasMaxLength(50);
            address.Property(a => a.State).HasMaxLength(50);
            address.Property(a => a.ZipCode).IsRequired().HasMaxLength(10);
        }
    }
}
