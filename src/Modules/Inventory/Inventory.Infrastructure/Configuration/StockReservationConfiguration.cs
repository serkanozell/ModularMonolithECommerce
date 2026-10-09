namespace Inventory.Infrastructure.Configuration
{
    public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
    {
        public void Configure(EntityTypeBuilder<StockReservation> builder)
        {
            builder.HasKey(reservation => reservation.Id);

            builder.Property(reservation => reservation.InventoryItemId)
                   .HasConversion(id => id.Value, value => InventoryItemId.Of(value))
                   .IsRequired();

            builder.Property(reservation => reservation.OrderId)
                   .HasConversion(orderId => orderId.Value, value => OrderId.Of(value))
                   .IsRequired();

            builder.Property(reservation => reservation.Quantity)
                   .HasConversion(quantity => quantity.Value, value => Quantity.Of(value))
                   .IsRequired();

            builder.Property(reservation => reservation.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasIndex(reservation => new { reservation.InventoryItemId, reservation.OrderId })
                   .IsUnique()
                   .HasFilter("\"is_active\" = TRUE AND \"is_deleted\" = FALSE");
        }
    }
}
