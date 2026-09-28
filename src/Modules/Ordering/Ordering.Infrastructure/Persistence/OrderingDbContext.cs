namespace Ordering.Infrastructure.Persistence
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ordering");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
