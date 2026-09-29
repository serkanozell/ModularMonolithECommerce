namespace Ordering.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_items");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                   .IsRequired();

            builder.Property(x => x.ProductId)
                   .IsRequired();

            builder.Property(x => x.Quantity)
                   .IsRequired();

            builder.Property(x => x.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");
        }
    }
}
