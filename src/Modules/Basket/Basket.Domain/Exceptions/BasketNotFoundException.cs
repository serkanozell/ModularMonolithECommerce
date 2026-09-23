namespace Basket.Domain.Exceptions
{
    public class BasketNotFoundException(string userName) : NotFoundException("ShoppingCart", userName)
    {
    }
}