/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
using System.Linq;
using System.Reflection;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("FluentAPI.CSharp", "System.Reflection", 4)]
    [APIDescriptionCN("针对 System.Reflection 提供的链式扩展")]
    [APIDescriptionEN("Chain extension provided for System.Reflection")]
#endif
    public static class SystemReflectionExtension
    {
        // [UnityEditor.MenuItem("QF/Test")]
        // public static void Test()
        // {
        //         "/abc/e.txt".GetFileExtendName().LogInfo();
        // }
        
#if UNITY_EDITOR
        // v1 No.32
        [MethodAPI]
        [APIDescriptionCN("通过反射的方式调用私有方法")]
        [APIDescriptionEN("call private method by reflection")]
        [APIExampleCode(@"
class A
{
    private void Say() { Debug.Log(""I'm A!"") }
}

new A().ReflectionCallPrivateMethod(""Say"");
// I'm A!
")]
#endif
        public static object ReflectionCallPrivateMethod<T>(this T self, string methodName, params object[] args)
        {
            var type = typeof(T);
            var methodInfo = type.GetMethod(methodName,BindingFlags.Instance | BindingFlags.NonPublic);
            
            return methodInfo?.Invoke(self, args);
        }

#if UNITY_EDITOR
        // v1 No.33
        [MethodAPI]
        [APIDescriptionCN("通过反射的方式调用私有方法，有返回值")]
        [APIDescriptionEN("call private method by reflection,return the result")]
        [APIExampleCode(@"
class A
{
    private bool Add(int a,int b) { return a + b; }
}

Debug.Log(new A().ReflectionCallPrivateMethod(""Add"",1,2));
// 3
")]
#endif
        public static TReturnType ReflectionCallPrivateMethod<T,TReturnType>(this T self, string methodName, params object[] args)
        {
            return (TReturnType)self.ReflectionCallPrivateMethod(methodName, args);
        }
        
          /// <summary>
        /// 通过反射方式调用函数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object InvokeByReflect(this object obj, string methodName, params object[] args)
        {
            var methodInfo = obj.GetType().GetMethod(methodName);
            return methodInfo == null ? null : methodInfo.Invoke(obj, args);
        }

        /// <summary>
        /// 通过反射方式获取域值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">域名</param>
        /// <returns></returns>
        public static object GetFieldByReflect(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            return fieldInfo == null ? null : fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// 通过反射方式获取属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyByReflect(this object obj, string propertyName, object[] index = null)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, index);
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo prop, Type attributeType, bool inherit)
        {
            return prop.GetCustomAttributes(attributeType, inherit).Any();
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this FieldInfo field, Type attributeType, bool inherit)
        {
            return field.GetCustomAttributes(attributeType, inherit).Any();
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit).Any();
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this MethodInfo method, Type attributeType, bool inherit)
        {
            return method.GetCustomAttributes(attributeType, inherit).Any();
        }


        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this MethodInfo method, bool inherit) where T : Attribute
        {
            var attrs = (T[])method.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this FieldInfo field, bool inherit) where T : Attribute
        {
            var attrs = (T[])field.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
        {
            var attrs = (T[])prop.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var attrs = (T[])type.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }
    }
}