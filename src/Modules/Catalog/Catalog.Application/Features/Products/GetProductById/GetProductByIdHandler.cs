namespace Catalog.Application.Features.Products.GetProductById
{
    internal sealed class GetProductByIdQueryHandler(IProductRepository repository) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            if (query.Id == Guid.Empty)
                throw new ArgumentException("Id is required.", nameof(query));

            var product = await repository.GetByIdAsync(query.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with id '{query.Id}' was not found.");

            return new GetProductByIdResult(product.ToDto());
        }
    }
}