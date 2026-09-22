namespace Catalog.Application.Features.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<ProductDto> Products);

    internal class GetProductByCategoryHandler(IProductRepository productRepository) : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            // get products by category using dbContext
            // return result

            var products = await productRepository.GetByCategoryAsync(query.Category, cancellationToken);

            //mapping product entity to productdto
            var productDtos = products.ToDtoList();

            return new GetProductByCategoryResult(productDtos);
        }
    }
}