namespace Ordering.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_items");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasConversion(id => id.Value, value => OrderItemId.Of(value));

            builder.Property(x => x.OrderId)
                   .HasConversion(id => id.Value, value => OrderId.Of(value))
                   .IsRequired();

            builder.Property(x => x.ProductId)
                   .HasConversion(id => id.Value, value => ProductId.Of(value))
                   .IsRequired();

            builder.Property(x => x.Quantity)
                   .HasConversion(quantity => quantity.Value, value => Quantity.Of(value))
                   .IsRequired();

            builder.Property(x => x.Price)
                   .IsRequired()
                   .HasConversion(price => price.Value, value => Price.Of(value))
                   .HasColumnType("decimal(18,2)");
        }
    }
}
