using System.Reflection;

namespace Ordering.Application
{
    public static class OrderingApplicationAssembly
    {
        public static Assembly Instance => typeof(OrderingApplicationAssembly).Assembly;
    }
}
