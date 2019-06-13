using System;
using System.Windows.Threading;

namespace Common
{
    public static class Extensions
    {
        public static void Invoke(this DispatcherObject obj, Action act)
        {
            obj.Dispatcher.Invoke(act);
        }

        public static void InvokeAsync(this DispatcherObject obj, Action act)
        {
            obj.Dispatcher.BeginInvoke(act);
        }

        public static void InvokeAsync(this DispatcherObject obj, Action act, DispatcherPriority priority)
        {
            obj.Dispatcher.BeginInvoke(act, priority);
        }
    }
}
