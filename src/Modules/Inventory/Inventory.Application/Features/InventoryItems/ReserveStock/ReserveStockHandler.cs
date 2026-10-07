namespace Inventory.Application.Features.InventoryItems.ReserveStock
{
    public record ReserveStockCommand(Guid Id, Guid OrderId, int Quantity) : ICommand<ReserveStockResult>;

    public record ReserveStockResult(bool IsSuccess);

    public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
    {
        public ReserveStockCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.Quantity).GreaterThan(0);
        }
    }

    internal sealed class ReserveStockCommandHandler(IInventoryItemRepository repository) : ICommandHandler<ReserveStockCommand, ReserveStockResult>
    {
        public async Task<ReserveStockResult> Handle(ReserveStockCommand command, CancellationToken cancellationToken)
        {
            var item = await repository.GetByIdWithReservationsAsync(command.Id, command.OrderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{command.Id}' was not found.");

            if (!item.Reserve(command.OrderId, command.Quantity))
                return new ReserveStockResult(false);

            try
            {
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine($"{entry.Metadata.ClrType.Name}: {entry.State}");
                }

                new ReserveStockResult(false);
            }

            return new ReserveStockResult(true);
        }
    }
}
