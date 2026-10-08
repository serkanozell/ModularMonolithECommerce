using Catalog.Contracts.Features.Products.UpdateProduct;

namespace Catalog.Application.Features.Products.UpdateProductStock
{
    internal sealed class UpdateProductStockCommandHandler(IProductRepository repository) : ICommandHandler<UpdateProductStockCommand, UpdateProductStockCommandResult>
    {
        public async Task<UpdateProductStockCommandResult> Handle(UpdateProductStockCommand command, CancellationToken cancellationToken)
        {
            var product = await repository.GetByIdAsync(command.ProductId, cancellationToken) ?? throw new KeyNotFoundException($"Product with id '{command.ProductId}' was not found.");

            product.UpdateStockQuantity(command.IsInStock, command.AvailableQuantity);

            repository.Update(product);

            await repository.SaveChangesAsync(cancellationToken);

            return new UpdateProductStockCommandResult(true);
        }
    }
}