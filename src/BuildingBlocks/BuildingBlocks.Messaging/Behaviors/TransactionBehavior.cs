using BuildingBlocks.Messaging.Persistence;
using BuildingBlocks.Messaging.Transactions;
using BuildingBlocks.Shared.CQRS;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace BuildingBlocks.Messaging.Behaviors
{
    /// <summary>
    /// Wraps every command in a single database transaction on the shared connection.
    /// Module DbContexts are enlisted via <see cref="EnlistTransactionInterceptor"/> and
    /// outbox messages written by IPublishEndpoint are persisted through <see cref="MessagingDbContext"/>.
    /// </summary>
    public sealed class TransactionBehavior<TRequest, TResponse>(
        DbConnection connection,
        ITransactionAccessor transactionAccessor,
        MessagingDbContext messagingDbContext,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICommand<TResponse>)
                return await next();

            // Nested command inside an already running transaction
            if (transactionAccessor.Current is not null)
                return await next();

            // Transaction started by MassTransit consumer outbox: join it, MassTransit commits
            if (messagingDbContext.Database.CurrentTransaction is { } externalTransaction)
            {
                transactionAccessor.Current = externalTransaction.GetDbTransaction();
                try
                {
                    return await next();
                }
                finally
                {
                    transactionAccessor.Current = null;
                }
            }

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            transactionAccessor.Current = transaction;

            try
            {
                var response = await next();

                await messagingDbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Transaction rolled back for {RequestName}", typeof(TRequest).Name);
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
            finally
            {
                transactionAccessor.Current = null;
            }
        }
    }
}
