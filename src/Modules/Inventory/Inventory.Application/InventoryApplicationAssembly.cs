using System.Reflection;

namespace Inventory.Application
{
    public static class InventoryApplicationAssembly
    {
        public static Assembly Instance => typeof(InventoryApplicationAssembly).Assembly;
    }
}