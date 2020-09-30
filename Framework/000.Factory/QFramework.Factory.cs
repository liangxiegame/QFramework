using System;
using System.Reflection;

namespace QFramework
{
    public class ObjectFactory
    {
        public static object Create(Type type, params object[] constructorArgs)
        {
            return Activator.CreateInstance(type, constructorArgs);
        }

        public static T Create<T>(params object[] constructorArgs)
        {
            return (T) Create(typeof(T), constructorArgs);
        }

        public static object CreateNonPublicConstructorObject(Type type)
        {
            // 获取私有构造函数
            var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            // 获取无参构造函数
            var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + type);
            }

            return ctor.Invoke(null);
        }

        public static T CreateNonPublicConstructorObject<T>()
        {
            return (T) CreateNonPublicConstructorObject(typeof(T));
        }


        public static object CreateWithInitialAction(Type type, Action<object> onObjectCreate,
            params object[] constructorArgs)
        {
            var obj = Create(type, constructorArgs);
            onObjectCreate(obj);
            return obj;
        }

        public static T CreateWithInitialAction<T>(Action<T> onObjectCreate,
            params object[] constructorArgs)
        {
            var obj = Create<T>(constructorArgs);
            onObjectCreate(obj);
            return obj;
        }
    }
}