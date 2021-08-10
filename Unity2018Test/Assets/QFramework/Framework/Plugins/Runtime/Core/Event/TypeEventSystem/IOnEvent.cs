using System;

namespace QFramework
{
    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }

    public static class OnEventExtension
    {
        public static IDisposable RegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            return TypeEventSystem.Register<T>(self.OnEvent);
        }

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            TypeEventSystem.UnRegister<T>(self.OnEvent);
        }
    }
}