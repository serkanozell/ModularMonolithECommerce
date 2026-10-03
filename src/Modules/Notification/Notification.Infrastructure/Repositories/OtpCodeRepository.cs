using Notification.Domain.Repositories;
using Notification.Infrastructure.Persistence;

namespace Notification.Infrastructure.Repositories
{
    public class OtpCodeRepository(NotificationDbContext context) : IOtpCodeRepository
    {
        public void Add(OtpCode otpCode) => context.OtpCodes.Add(otpCode);

        public Task<OtpCode?> GetLatestActiveAsync(string recipient, string purpose, CancellationToken cancellationToken = default) =>
            ActiveCodes(recipient, purpose)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<OtpCode>> GetActiveAsync(string recipient, string purpose, CancellationToken cancellationToken = default) =>
            await ActiveCodes(recipient, purpose).ToListAsync(cancellationToken);

        public Task<int> CountCreatedSinceAsync(string recipient, DateTime sinceUtc, CancellationToken cancellationToken = default) =>
            context.OtpCodes.CountAsync(x => x.Recipient == recipient && x.CreatedAt >= sinceUtc, cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);

        private IQueryable<OtpCode> ActiveCodes(string recipient, string purpose) =>
            context.OtpCodes.Where(x => x.Recipient == recipient && x.Purpose == purpose && x.IsActive && !x.IsDeleted);
    }
}
