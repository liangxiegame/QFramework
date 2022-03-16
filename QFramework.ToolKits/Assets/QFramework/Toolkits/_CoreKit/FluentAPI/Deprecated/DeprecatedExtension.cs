/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.IO;

namespace QFramework
{
    public static class DeprecatedExtension
    {
        /// <summary>
        /// 检测路径是否存在，如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used",true)]
        public static string CreateDirIfNotExists4FilePath(this string path)
        {
            var direct = path.GetPathParentFolder();

            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }

            return path;
        }

        
        
        /// <summary>
        /// 获取泛型名字
        /// <code>
        /// var typeName = GenericExtention.GetTypeName<string>();
        /// typeName.LogInfo(); // string
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used",true)]
        public static string GetTypeName<T>()
        {
            return typeof(T).ToString();
        }
        
        
        [Obsolete("不要使用，Do not used",true)]
        public static void DoIfNotNull<T>(this T selfObj, Action<T> action) where T : class
        {
            if (selfObj != null)
            {
                action(selfObj);
            }
        }
        
        
        /// <summary>
        /// 是否相等
        /// 
        /// 示例：
        /// <code>
        /// if (this.Is(player))
        /// {
        ///     ...
        /// }
        /// </code>
        /// </summary>
        /// <param name="selfObj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("请使用 Object.Equals(A,B)，please use Object.Equals() isntead",true)]
        public static bool Is<T>(this T selfObj, T value)
        {
            return Equals(selfObj, value);
        }

        [Obsolete("请使用 Object.Equals(A,B)，please use Object.Equals() isntead",true)]
        public static bool Is<T>(this T selfObj, Func<T, bool> condition)
        {
            return condition(selfObj);
        }

        /// <summary>
        /// 表达式成立 则执行 Action
        /// 
        /// 示例:
        /// <code>
        /// (1 == 1).Do(()=>Debug.Log("1 == 1");
        /// </code>
        /// </summary>
        /// <param name="selfCondition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used",true)]
        public static bool Do(this bool selfCondition, Action action)
        {
            if (selfCondition)
            {
                action();
            }

            return selfCondition;
        }

        /// <summary>
        /// 不管表达成不成立 都执行 Action，并把结果返回
        /// 
        /// 示例:
        /// <code>
        /// (1 == 1).Do((result)=>Debug.Log("1 == 1:" + result);
        /// </code>
        /// </summary>
        /// <param name="selfCondition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used",true)]
        public static bool Do(this bool selfCondition, Action<bool> action)
        {
            action(selfCondition);

            return selfCondition;
        }
        
        #region Func Extension

        /// <summary>
        /// 功能：不为空则调用 Func
        /// 
        /// 示例:
        /// <code>
        /// Func<int> func = ()=> 1;
        /// var number = func.InvokeGracefully(); // 等价于 if (func != null) number = func();
        /// </code>
        /// </summary>
        /// <param name="selfFunc"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead",true)]
        public static T InvokeGracefully<T>(this Func<T> selfFunc)
        {
            return null != selfFunc ? selfFunc() : default(T);
        }

        #endregion

        #region Action

        /// <summary>
        /// 功能：不为空则调用 Action
        /// 
        /// 示例:
        /// <code>
        /// System.Action action = () => Log.I("action called");
        /// action.InvokeGracefully(); // if (action != null) action();
        /// </code>
        /// </summary>
        /// <param name="selfAction"> action 对象 </param>
        /// <returns> 是否调用成功 </returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead",true)]
        public static bool InvokeGracefully(this Action selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用 Action<T>
        /// 
        /// 示例:
        /// <code>
        /// System.Action<int> action = (number) => Log.I("action called" + number);
        /// action.InvokeGracefully(10); // if (action != null) action(10);
        /// </code>
        /// </summary>
        /// <param name="selfAction"> action 对象</param>
        /// <typeparam name="T">参数</typeparam>
        /// <returns> 是否调用成功</returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead",true)]
        public static bool InvokeGracefully<T>(this Action<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用 Action<T,K>
        ///
        /// 示例
        /// <code>
        /// System.Action<int,string> action = (number,name) => Log.I("action called" + number + name);
        /// action.InvokeGracefully(10,"qframework"); // if (action != null) action(10,"qframework");
        /// </code>
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead",true)]
        public static bool InvokeGracefully<T, K>(this Action<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用委托
        ///
        /// 示例：
        /// <code>
        /// // delegate
        /// TestDelegate testDelegate = () => { };
        /// testDelegate.InvokeGracefully();
        /// </code>
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call suceed </returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead",true)]
        public static bool InvokeGracefully(this Delegate selfAction, params object[] args)
        {
            if (null != selfAction)
            {
                selfAction.DynamicInvoke(args);
                return true;
            }

            return false;
        }

        #endregion
    }
}