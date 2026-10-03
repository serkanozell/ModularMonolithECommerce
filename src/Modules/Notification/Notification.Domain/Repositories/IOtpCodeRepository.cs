using Notification.Domain.Entities;

namespace Notification.Domain.Repositories
{
    public interface IOtpCodeRepository
    {
        void Add(OtpCode otpCode);
        Task<OtpCode?> GetLatestActiveAsync(string recipient, string purpose, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OtpCode>> GetActiveAsync(string recipient, string purpose, CancellationToken cancellationToken = default);
        Task<int> CountCreatedSinceAsync(string recipient, DateTime sinceUtc, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}