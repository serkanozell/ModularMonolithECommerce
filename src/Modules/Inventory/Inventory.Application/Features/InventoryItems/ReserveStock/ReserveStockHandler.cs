using BuildingBlocks.Shared.Dtos;

namespace Inventory.Application.Features.InventoryItems.ReserveStock
{
    public record ReserveStockCommand(List<OrderItemDto> OrderItems, Guid OrderId) : ICommand<ReserveStockResult>;

    public record ReserveStockResult(bool IsSuccess, string? FailureReason = null);

    public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
    {
        public ReserveStockCommandValidator()
        {
            RuleFor(command => command.OrderItems).NotEmpty();
            RuleFor(command => command.OrderId).NotEmpty();
            RuleForEach(command => command.OrderItems).ChildRules(item =>
            {
                item.RuleFor(orderItem => orderItem.ProductId).NotEmpty();
                item.RuleFor(orderItem => orderItem.Quantity).GreaterThan(0);
            });
        }
    }

    internal sealed class ReserveStockCommandHandler(IInventoryItemRepository repository) : ICommandHandler<ReserveStockCommand, ReserveStockResult>
    {
        public async Task<ReserveStockResult> Handle(ReserveStockCommand command, CancellationToken cancellationToken)
        {
            foreach (var orderItem in command.OrderItems)
            {
                var inventoryItem = await repository.GetByProductIdWithReservationsAsync(orderItem.ProductId, command.OrderId, cancellationToken)
                    ?? throw new KeyNotFoundException($"Inventory item for product '{orderItem.ProductId}' was not found.");

                if (orderItem.Quantity > inventoryItem.StockLevel.Available)
                    return new ReserveStockResult(false, $"Insufficient stock for product '{orderItem.ProductId}'.");

                if (!inventoryItem.Reserve(command.OrderId, orderItem.Quantity))
                    return new ReserveStockResult(false, $"Stock reservation was rejected for product '{orderItem.ProductId}'.");
            }

            try
            {
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return new ReserveStockResult(false, "Stock changed during the reservation attempt. Please retry.");
            }

            return new ReserveStockResult(true);
        }
    }
}
