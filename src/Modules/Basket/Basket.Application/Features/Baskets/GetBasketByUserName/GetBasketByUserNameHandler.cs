using Basket.Application.Features.Baskets.Dtos;
using Basket.Domain.Exceptions;

namespace Basket.Application.Features.Baskets.GetBasketByUserName
{
    public record GetBasketByUserNameQuery(string UserName) : IQuery<GetBasketByUserNameResult>;

    public record GetBasketByUserNameResult(BasketDto Basket);

    internal sealed class GetBasketByUserNameQueryHandler(IBasketRepository repository) : IQueryHandler<GetBasketByUserNameQuery, GetBasketByUserNameResult>
    {
        public async Task<GetBasketByUserNameResult> Handle(GetBasketByUserNameQuery query, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(query.UserName);

            var basket = await repository.GetBasket(query.UserName, true, cancellationToken)
                         ?? throw new BasketNotFoundException($"Basket for '{query.UserName}' was not found.");

            return new GetBasketByUserNameResult(basket.ToDto());
        }
    }
}