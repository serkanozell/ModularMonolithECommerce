namespace Catalog.Application.Features.Products.GetProducts
{
    public record GetProductsQuery(PaginationRequest PaginationRequest) : IQuery<GetProductsResult>;

    public record GetProductsResult(PaginatedResult<ProductDto> Products);

    public class GetProductsQueryHandler(IProductRepository repository) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        private const int MaxPageSize = 100;

        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex < 0 ? 0 : query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize < 1 ? 10 : Math.Min(query.PaginationRequest.PageSize, MaxPageSize);

            var totalCount = await repository.CountAsync(cancellationToken);
            var products = await repository.GetPagedAsync(pageIndex, pageSize, cancellationToken);

            return new GetProductsResult(new PaginatedResult<ProductDto>(pageIndex, pageSize, totalCount, products.ToDtoList()));
        }
    }
}