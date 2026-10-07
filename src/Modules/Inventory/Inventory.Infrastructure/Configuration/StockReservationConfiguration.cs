namespace Inventory.Infrastructure.Configuration
{
    public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
    {
        public void Configure(EntityTypeBuilder<StockReservation> builder)
        {
            builder.HasKey(reservation => reservation.Id);

            builder.Property(reservation => reservation.OrderId)
                   .IsRequired();

            builder.Property(reservation => reservation.Quantity)
                   .IsRequired();

            builder.Property(reservation => reservation.Status)
                   .IsRequired();

            builder.HasIndex(reservation => new { reservation.InventoryItemId, reservation.OrderId })
                   .IsUnique()
                   .HasFilter("\"is_active\" = TRUE AND \"is_deleted\" = FALSE");
        }
    }
}
