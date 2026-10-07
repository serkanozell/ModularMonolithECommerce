namespace Inventory.Application.Features.InventoryItems.IncreaseStock
{
    public record IncreaseStockCommand(Guid Id, int Quantity) : ICommand<IncreaseStockResult>;

    public record IncreaseStockResult(bool IsSuccess);

    public class IncreaseStockCommandValidator : AbstractValidator<IncreaseStockCommand>
    {
        public IncreaseStockCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.Quantity).GreaterThan(0);
        }
    }

    internal sealed class IncreaseStockCommandHandler(IInventoryItemRepository repository) : ICommandHandler<IncreaseStockCommand, IncreaseStockResult>
    {
        public async Task<IncreaseStockResult> Handle(IncreaseStockCommand command, CancellationToken cancellationToken)
        {
            var item = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{command.Id}' was not found.");

            item.IncreaseStock(command.Quantity);
            repository.Update(item);
            await repository.SaveChangesAsync(cancellationToken);

            return new IncreaseStockResult(true);
        }
    }
}