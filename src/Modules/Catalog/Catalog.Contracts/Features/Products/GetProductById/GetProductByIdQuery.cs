using BuildingBlocks.Shared.CQRS;

namespace Catalog.Contracts.Features.Products.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

    public record GetProductByIdResult(ProductDto Product);
}
