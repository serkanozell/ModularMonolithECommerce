using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Messaging.Persistence
{
    public class MessagingDbContext(DbContextOptions<MessagingDbContext> options) : DbContext(options)
    {
        public const string Schema = "messaging";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            base.OnModelCreating(modelBuilder);
        }
    }
}
