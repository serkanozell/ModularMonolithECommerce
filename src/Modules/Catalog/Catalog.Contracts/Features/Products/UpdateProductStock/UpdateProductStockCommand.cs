using BuildingBlocks.Shared.CQRS;

namespace Catalog.Contracts.Features.Products.UpdateProduct
{
    public record UpdateProductStockCommand(
        Guid ProductId,
        bool IsInStock,
        int AvailableQuantity,
        DateTime OccurredOnUtc) : ICommand<UpdateProductStockCommandResult>;

    public record UpdateProductStockCommandResult(bool IsSuccess);
}