/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QFramework
{
    /// <summary>
    /// 简单的概率计算
    /// </summary>
    public static class ProbilityHelper
    {
        [Obsolete("不要使用，Do not used", true)]
        public static T RandomValueFrom<T>(params T[] values)
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        /// <summary>
        /// percent probability
        /// </summary>
        /// <param name="percent"> 0 ~ 100 </param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", true)]
        public static bool PercentProbability(int percent)
        {
            return UnityEngine.Random.Range(0, 1000) * 0.001f < 50 * 0.01f;
        }
    }

    /// <summary>
    /// Write in unity 2017 .Net 3.5
    /// after unity 2018 .Net 4.x and new C# version are more powerful
    /// </summary>
    public static class DeprecatedExtension
    {
        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        [Obsolete("不要使用，Do not used", true)]
        public static bool ImplementsInterface<T>(this Type type)
        {
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }

        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        [Obsolete("不要使用，Do not used", true)]
        public static bool ImplementsInterface<T>(this object obj)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }


        /// <summary>
        /// 使目录存在,Path可以是目录名必须是文件名
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", true)]
        public static void MakeFileDirectoryExist(string path)
        {
            string root = Path.GetDirectoryName(path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }


        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", true)]
        public static string GetFolderName(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }

        // "使路径标准化，去除空格并将所有'\'转换为'/'"
        // "Normalize paths by removing whitespace and converting all '\' to '/'"
        [Obsolete("不要使用，Do not used", true)]
        public static string MakePathStandard(string path)
        {
            return path.Trim().Replace("\\", "/");
        }


        /// <summary>
        /// 获取不带后缀的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        [Obsolete("不要使用，Do not used", true)]
        public static string GetFilePathWithoutExtension(string fileName)
        {
            if (fileName.Contains("."))
                return fileName.Substring(0, fileName.LastIndexOf('.'));
            return fileName;
        }

        /// <summary>
        /// 获取不带后缀的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", true)]
        public static string GetFileNameWithoutExtension(string fileName, char separator = '/')
        {
            return GetFilePathWithoutExtension(GetFileName(fileName, separator));
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", true)]
        public static string GetFileName(string path, char separator = '/')
        {
            path = MakePathStandard(path);
            return path.Substring(path.LastIndexOf(separator) + 1);
        }

        /// <summary>
        /// 获取文件夹名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", true)]
        public static string GetDirectoryName(string fileName)
        {
            fileName = MakePathStandard(fileName);
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }


        [Obsolete("不要使用，Do not used", true)]
        public static string GetDirPath(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        [Obsolete("不要使用，Do not used", true)]
        public static string GetLastDirName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return absOrAssetsPath.EndsWith("/") ? dirs[dirs.Length - 2] : dirs[dirs.Length - 1];
        }

        [Obsolete("不要使用，Do not used", true)]
        public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true,
            string suffix = "")
        {
            var pathList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                var dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        [Obsolete("不要使用，Do not used", true)]
        public static List<string> GetDirSubDirNameList(this string dirABSPath)
        {
            var di = new DirectoryInfo(dirABSPath);

            var dirs = di.GetDirectories();

            return dirs.Select(d => d.Name).ToList();
        }

        /// <summary>
        /// 使目录存在
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", true)]
        public static void MakeDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", true)]
        public static void OpenFolder(string path)
        {
#if UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", path);
#endif
        }

        /// <summary>
        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", true)]
        public static string GetPathParentFolder(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 检测路径是否存在，如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", true)]
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
        [Obsolete("不要使用，Do not used", true)]
        public static string GetTypeName<T>()
        {
            return typeof(T).ToString();
        }


        [Obsolete("不要使用，Do not used", true)]
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
        [Obsolete("请使用 Object.Equals(A,B)，please use Object.Equals() isntead", true)]
        public static bool Is<T>(this T selfObj, T value)
        {
            return Equals(selfObj, value);
        }

        [Obsolete("请使用 Object.Equals(A,B)，please use Object.Equals() isntead", true)]
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
        [Obsolete("不要使用，Do not used", true)]
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
        [Obsolete("不要使用，Do not used", true)]
        public static bool Do(this bool selfCondition, Action<bool> action)
        {
            action(selfCondition);

            return selfCondition;
        }

        

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
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", true)]
        public static T InvokeGracefully<T>(this Func<T> selfFunc)
        {
            return null != selfFunc ? selfFunc() : default(T);
        }
        

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
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", true)]
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
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", true)]
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
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", true)]
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
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", true)]
        public static bool InvokeGracefully(this Delegate selfAction, params object[] args)
        {
            if (null != selfAction)
            {
                selfAction.DynamicInvoke(args);
                return true;
            }

            return false;
        }
    }
}