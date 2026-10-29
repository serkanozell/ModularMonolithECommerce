using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using Payment.Domain.ValueObjects;
using Payment.Infrastructure.Persistance;

namespace Payment.Infrastructure.Repositories
{
    public class PaymentTransactionRepository(PaymentDbContext context) : IPaymentTransactionRepository
    {
        public void Add(PaymentTransaction transaction) => context.PaymentTransactions.Add(transaction);

        public async Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentTransactions.FirstOrDefaultAsync(p => p.OrderId == OrderId.Of(orderId), cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken);
    }
}