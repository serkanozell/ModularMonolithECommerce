using Microsoft.EntityFrameworkCore;
using Ordering.Application.Features.Orders.Sagas;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderingSagaDbContext(DbContextOptions<OrderingSagaDbContext> options) : DbContext(options)
    {
        public DbSet<OrderCheckoutSagaState> OrderCheckoutSagaStates => Set<OrderCheckoutSagaState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ordering");

            modelBuilder.Entity<OrderCheckoutSagaState>(entity =>
            {
                entity.ToTable("order_checkout_saga_states");
                entity.HasKey(state => state.CorrelationId);
                entity.Property(state => state.CurrentState).IsRequired().HasMaxLength(64);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}