namespace Ordering.Application.Features.Orders.ConfirmOrder
{
    public record ConfirmOrderCommand(Guid OrderId) : ICommand<ConfirmOrderResult>;

    public record ConfirmOrderResult(bool IsSuccess);

    public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
    {
        public ConfirmOrderCommandValidator()
        {
            RuleFor(o => o.OrderId).NotEmpty();
        }
    }

    public sealed class ConfirmOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<ConfirmOrderCommand, ConfirmOrderResult>
    {
        public async Task<ConfirmOrderResult> Handle(ConfirmOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken) ?? throw new KeyNotFoundException($"Order with ID : {command.OrderId}");

            order.Confirm();

            await orderRepository.SaveChangesAsync(cancellationToken);

            return new ConfirmOrderResult(true);
        }
    }
}