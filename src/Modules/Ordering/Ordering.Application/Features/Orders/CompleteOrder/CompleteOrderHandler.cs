namespace Ordering.Application.Features.Orders.CompleteOrder
{
    public record CompleteOrderCommand(Guid OrderId) : ICommand<CompleteOrderResult>;

    public record CompleteOrderResult(bool IsSuccess);

    public class CompleteOrderCommandValidator : AbstractValidator<CompleteOrderCommand>
    {
        public CompleteOrderCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
        }
    }

    internal sealed class CompleteOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<CompleteOrderCommand, CompleteOrderResult>
    {
        public async Task<CompleteOrderResult> Handle(CompleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with ID '{command.OrderId}' was not found.");

            order.Complete();

            await orderRepository.SaveChangesAsync(cancellationToken);

            return new CompleteOrderResult(true);
        }
    }
}