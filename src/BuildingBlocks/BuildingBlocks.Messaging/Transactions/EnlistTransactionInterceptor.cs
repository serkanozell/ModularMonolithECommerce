using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Messaging.Transactions
{
    /// <summary>
    /// Enlists every DbContext sharing the scoped connection into the ambient transaction
    /// opened by TransactionBehavior, so that module data and outbox messages commit atomically.
    /// </summary>
    public sealed class EnlistTransactionInterceptor(ITransactionAccessor transactionAccessor) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Enlist(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Enlist(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void Enlist(DbContext? context)
        {
            var transaction = transactionAccessor.Current;

            if (context is null || transaction is null || context.Database.CurrentTransaction is not null)
                return;

            context.Database.UseTransaction(transaction);
        }
    }
}
