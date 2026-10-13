namespace Ordering.Application.Features.Orders.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId, string Reason) : ICommand<CancelOrderResult>;

    public record CancelOrderResult(bool IsSuccess);

    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.Reason).NotEmpty().MaximumLength(500);
        }
    }

    internal sealed class CancelOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<CancelOrderCommand, CancelOrderResult>
    {
        public async Task<CancelOrderResult> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with ID '{command.OrderId}' was not found.");

            order.Cancel(command.Reason);

            await orderRepository.SaveChangesAsync(cancellationToken);

            return new CancelOrderResult(true);
        }
    }
}