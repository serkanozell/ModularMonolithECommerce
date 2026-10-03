namespace Catalog.Application.Features.Products.GetProducts
{
    public record GetProductsQuery(PaginationRequest PaginationRequest) : IQuery<GetProductsResult>;

    public record GetProductsResult(PaginatedResult<ProductDto> Products);

    internal sealed class GetProductsQueryHandler(IProductRepository repository) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        private const int MaxPageSize = 100;

        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = query.PaginationRequest.PageNumber < 0 ? 0 : query.PaginationRequest.PageNumber;
            var pageSize = query.PaginationRequest.PageSize < 1 ? 10 : Math.Min(query.PaginationRequest.PageSize, MaxPageSize);

            var totalCount = await repository.CountAsync(cancellationToken);
            var products = await repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

            return new GetProductsResult(new PaginatedResult<ProductDto>(pageNumber, pageSize, totalCount, products.ToDtoList()));
        }
    }
}