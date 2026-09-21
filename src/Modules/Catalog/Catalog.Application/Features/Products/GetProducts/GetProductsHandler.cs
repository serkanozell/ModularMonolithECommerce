namespace Catalog.Application.Features.Products.GetProducts
{
    public record GetProductsQuery(int PageNumber, int PageSize) : IQuery<GetProductsResult>;

    public record GetProductsResult(int PageNumber,
                                    int PageSize,
                                    int TotalCount,
                                    List<ProductDto> Products);

    public class GetProductsQueryHandler(IProductRepository repository) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        private const int MaxPageSize = 100;

        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize < 1 ? 10 : Math.Min(query.PageSize, MaxPageSize);

            var totalCount = await repository.CountAsync(cancellationToken);
            var products = await repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

            return new GetProductsResult(pageNumber, pageSize, totalCount, products.ToDtoList());
        }
    }
}