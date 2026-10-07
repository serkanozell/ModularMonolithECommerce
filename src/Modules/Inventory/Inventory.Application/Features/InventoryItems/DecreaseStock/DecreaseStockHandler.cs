namespace Inventory.Application.Features.InventoryItems.DecreaseStock
{
    public record DecreaseStockCommand(Guid Id, int Quantity) : ICommand<DecreaseStockResult>;

    public record DecreaseStockResult(bool IsSuccess);

    public class DecreaseStockCommandValidator : AbstractValidator<DecreaseStockCommand>
    {
        public DecreaseStockCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.Quantity).GreaterThan(0);
        }
    }

    internal sealed class DecreaseStockCommandHandler(IInventoryItemRepository repository) : ICommandHandler<DecreaseStockCommand, DecreaseStockResult>
    {
        public async Task<DecreaseStockResult> Handle(DecreaseStockCommand command, CancellationToken cancellationToken)
        {
            var item = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{command.Id}' was not found.");

            item.DecreaseStock(command.Quantity);
            repository.Update(item);
            await repository.SaveChangesAsync(cancellationToken);

            return new DecreaseStockResult(true);
        }
    }
}