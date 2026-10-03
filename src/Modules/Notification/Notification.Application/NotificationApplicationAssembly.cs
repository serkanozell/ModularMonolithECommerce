using System.Reflection;

namespace Notification.Application
{
    public static class NotificationApplicationAssembly
    {
        public static Assembly Instance => typeof(NotificationApplicationAssembly).Assembly;
    }
}
