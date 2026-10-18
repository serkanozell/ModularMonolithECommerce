using BuildingBlocks.Shared.CQRS;
using FluentValidation;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Repositories;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Features.ProcessPayment
{
    public record ProcessPaymentCommand(Guid OrderId, decimal Amount) : ICommand<ProcessPaymentResult>;
    public record ProcessPaymentResult(bool IsSuccess, string? FailureReason = null);

    public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }

    internal sealed class ProcessPaymentCommandHandler(IPaymentTransactionRepository repository) : ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>
    {
        public async Task<ProcessPaymentResult> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
        {
            // Consumer retry / duplicate mesaj: aynı sipariş için ikinci kez ücret alınmaz.
            var existing = await repository.GetByOrderIdAsync(command.OrderId, cancellationToken);
            if (existing is not null)
                return existing.Status is PaymentStatus.Succeeded
                                        ? new ProcessPaymentResult(true)
                                        : new ProcessPaymentResult(false, existing.FailureReason ?? "Payment was already processed.");

            var transaction = PaymentTransaction.Create(OrderId.Of(command.OrderId), command.Amount);

            if (command.Amount > 5000)
                transaction.MarkFailed("Payment failed.");
            else
                transaction.MarkSucceeded();

            repository.Add(transaction);
            await repository.SaveChangesAsync(cancellationToken);

            return new ProcessPaymentResult(transaction.Status is PaymentStatus.Succeeded, transaction.FailureReason);
        }
    }
}