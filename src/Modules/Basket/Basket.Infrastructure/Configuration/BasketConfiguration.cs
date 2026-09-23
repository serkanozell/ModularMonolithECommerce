namespace Basket.Infrastructure.Configuration
{
    public class BasketConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            //builder.ToTable("ShoppingCarts");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(c => c.UserName);

            builder.Ignore(c => c.TotalPrice);

            builder.HasMany(c => c.Items)
                   .WithOne()
                   .HasForeignKey(i => i.ShoppingCartId)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.Metadata
            //       .FindNavigation(nameof(ShoppingCart.Items))!
            //       .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
