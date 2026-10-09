namespace Inventory.Infrastructure.Configuration
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            //builder.ToTable("inventory_items");

            builder.Property(item => item.Id)
                   .HasConversion(id => id.Value, value => InventoryItemId.Of(value));

            builder.HasKey(item => item.Id);

            builder.Property(item => item.ProductId)
                   .HasConversion(id => id.Value, value => ProductId.Of(value))
                   .IsRequired();

            builder.HasIndex(item => item.ProductId)
                   .IsUnique();

            builder.Property<uint>("xmin")
                   .IsRowVersion();

            builder.HasMany(item => item.Reservations)
                   .WithOne()
                   .HasForeignKey(reservation => reservation.InventoryItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(item => item.Reservations)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsOne(item => item.StockLevel, stockLevel =>
            {
                stockLevel.Property(level => level.OnHand)
                          .IsRequired();

                stockLevel.Property(level => level.Reserved)
                          .IsRequired();
            });

            builder.Navigation(item => item.StockLevel)
                   .IsRequired();

            builder.Ignore(item => item.IsInStock);
            builder.Ignore(item => item.DomainEvents);
            builder.HasQueryFilter(item => !item.IsDeleted);
        }
    }
}