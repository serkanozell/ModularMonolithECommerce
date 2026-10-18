using Payment.Domain.Entities;

namespace Payment.Domain.Repositories
{
    public interface IPaymentTransactionRepository
    {
        void Add(PaymentTransaction transaction);
        Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}