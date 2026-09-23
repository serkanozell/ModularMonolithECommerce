namespace Basket.Infrastructure.Configuration
{
    public class BasketItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            //builder.ToTable("ShoppingCartItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ShoppingCartId)
                   .IsRequired();

            builder.Property(i => i.ProductId)
                   .IsRequired();

            builder.Property(i => i.Quantity)
                   .IsRequired();

            builder.Property(i => i.Color)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(i => i.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.ProductName)
                   .IsRequired()
                   .HasMaxLength(200);
        }
    }
}
