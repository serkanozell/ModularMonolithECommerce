using System.Reflection;

namespace Payment.Application
{
    public static class PaymentApplicationAssembly
    {
        public static Assembly Instance => typeof(PaymentApplicationAssembly).Assembly;
    }
}