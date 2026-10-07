namespace Inventory.Application.Features.InventoryItems.CreateInventoryItem
{
    public record CreateInventoryItemCommand(Guid ProductId, int InitialQuantity) : ICommand<CreateInventoryItemResult>;

    public record CreateInventoryItemResult(Guid Id);

    public class CreateInventoryItemCommandValidator : AbstractValidator<CreateInventoryItemCommand>
    {
        public CreateInventoryItemCommandValidator()
        {
            RuleFor(command => command.ProductId).NotEmpty();
            RuleFor(command => command.InitialQuantity).GreaterThanOrEqualTo(0);
        }
    }

    internal sealed class CreateInventoryItemCommandHandler(IInventoryItemRepository repository) : ICommandHandler<CreateInventoryItemCommand, CreateInventoryItemResult>
    {
        public async Task<CreateInventoryItemResult> Handle(CreateInventoryItemCommand command, CancellationToken cancellationToken)
        {
            if (await repository.GetByProductIdAsync(command.ProductId, cancellationToken) is not null)
                throw new InvalidOperationException($"Inventory already exists for product '{command.ProductId}'.");

            var item = InventoryItem.Create(command.ProductId, command.InitialQuantity);
            repository.Add(item);
            await repository.SaveChangesAsync(cancellationToken);

            return new CreateInventoryItemResult(item.Id);
        }
    }
}