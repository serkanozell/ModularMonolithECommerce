namespace Basket.Infrastructure.Persistence
{
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options)
        {
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("basket");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BasketDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
