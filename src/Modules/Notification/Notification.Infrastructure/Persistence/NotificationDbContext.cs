namespace Notification.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("notification");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
