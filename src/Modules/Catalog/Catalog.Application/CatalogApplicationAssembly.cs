using System.Reflection;

namespace Catalog.Application
{
    public static class CatalogApplicationAssembly
    {
        public static Assembly Instance => typeof(CatalogApplicationAssembly).Assembly;
    }
}