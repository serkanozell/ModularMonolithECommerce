using System.Reflection;

namespace Basket.Application
{
    public static class BasketApplicationAssembly
    {
        public static Assembly Instance => typeof(BasketApplicationAssembly).Assembly;
    }
}
