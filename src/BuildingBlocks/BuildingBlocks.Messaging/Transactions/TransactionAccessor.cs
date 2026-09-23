using System.Data.Common;

namespace BuildingBlocks.Messaging.Transactions
{
    public interface ITransactionAccessor
    {
        DbTransaction? Current { get; set; }
    }

    internal sealed class TransactionAccessor : ITransactionAccessor
    {
        public DbTransaction? Current { get; set; }
    }
}
