using BuildingBlocks.Shared.CQRS;
using Payment.Domain.Repositories;

namespace Payment.Application.Features.RefundPayment
{
    public record RefundPaymentCommand(Guid OrderId) : ICommand<RefundPaymentResult>;
    public record RefundPaymentResult(bool IsSuccess);

    internal sealed class RefundPaymentCommandHandler(IPaymentTransactionRepository repository) : ICommandHandler<RefundPaymentCommand, RefundPaymentResult>
    {
        public async Task<RefundPaymentResult> Handle(RefundPaymentCommand command, CancellationToken cancellationToken)
        {
            var transaction = await repository.GetByOrderIdAsync(command.OrderId, cancellationToken);
            if (transaction is null) return new RefundPaymentResult(true); // iade edilecek bir şey yok

            transaction.Refund();
            await repository.SaveChangesAsync(cancellationToken);
            return new RefundPaymentResult(true);
        }
    }
}