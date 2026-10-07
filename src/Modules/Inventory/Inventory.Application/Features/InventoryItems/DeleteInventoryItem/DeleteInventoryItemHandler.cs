namespace Inventory.Application.Features.InventoryItems.DeleteInventoryItem
{
    public record DeleteInventoryItemCommand(Guid Id) : ICommand<DeleteInventoryItemResult>;

    public record DeleteInventoryItemResult(bool IsSuccess);

    public class DeleteInventoryItemCommandValidator : AbstractValidator<DeleteInventoryItemCommand>
    {
        public DeleteInventoryItemCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
        }
    }

    internal sealed class DeleteInventoryItemCommandHandler(IInventoryItemRepository repository) : ICommandHandler<DeleteInventoryItemCommand, DeleteInventoryItemResult>
    {
        public async Task<DeleteInventoryItemResult> Handle(DeleteInventoryItemCommand command, CancellationToken cancellationToken)
        {
            var item = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{command.Id}' was not found.");

            item.Delete();
            repository.Update(item);
            await repository.SaveChangesAsync(cancellationToken);

            return new DeleteInventoryItemResult(true);
        }
    }
}