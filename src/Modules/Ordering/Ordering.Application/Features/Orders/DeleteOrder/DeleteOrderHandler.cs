namespace Ordering.Application.Features.Orders.DeleteOrder
{
    public record DeleteOrderCommand(Guid Id) : ICommand<DeleteOrderResult>;

    public record DeleteOrderResult(bool IsSuccess);

    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }

    internal sealed class DeleteOrderCommandHandler(IOrderRepository repository) : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id '{command.Id}' was not found.");

            order.Delete();

            await repository.SaveChangesAsync(cancellationToken);

            return new DeleteOrderResult(true);
        }
    }
}
