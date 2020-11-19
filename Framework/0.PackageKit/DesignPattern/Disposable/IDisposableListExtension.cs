using System;

namespace QFramework
{
    public static class IDisposableExtensions
    {
        public static void AddTo(this IDisposable self, IDisposableList component)
        {
            component.Add(self);
        }
    }
    
}