/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this sender code.
 *
 * Document: http://catlib.io/
 */

using System;

namespace CatLib
{
    /// <summary>
    /// 应用程序接口
    /// </summary>
    public interface IApplication : IContainer, IDispatcher
    {
        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="provider">服务提供者</param>
        void Register(IServiceProvider provider);

        /// <summary>
        /// 服务提供者是否已经注册过
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <returns>服务提供者是否已经注册过</returns>
        bool IsRegisted(IServiceProvider provider);

        /// <summary>
        /// 获取程序运行时唯一Id
        /// </summary>
        /// <returns>运行时的唯一Id</returns>
        long GetRuntimeId();

        /// <summary>
        /// 是否是主线程
        /// </summary>
        bool IsMainThread { get; }

        /// <summary>
        /// CatLib版本(遵循semver)
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 比较CatLib版本(遵循semver)
        /// <para>输入版本大于当前版本则返回<code>-1</code></para>
        /// <para>输入版本等于当前版本则返回<code>0</code></para>
        /// <para>输入版本小于当前版本则返回<code>1</code></para>
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="revised">修订版本号</param>
        /// <returns>比较结果</returns>
        int Compare(int major, int minor, int revised);

        /// <summary>
        /// 比较CatLib版本(遵循semver)
        /// <para>输入版本大于当前版本则返回<code>-1</code></para>
        /// <para>输入版本等于当前版本则返回<code>0</code></para>
        /// <para>输入版本小于当前版本则返回<code>1</code></para>
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns>比较结果</returns>
        int Compare(string version);

        /// <summary>
        /// 获取优先级，如果存在方法优先级定义那么优先返回方法的优先级
        /// 如果不存在优先级定义那么返回<c>int.MaxValue</c>
        /// </summary>
        /// <param name="type">获取优先级的类型</param>
        /// <param name="method">获取优先级的调用方法</param>
        /// <returns>优先级</returns>
        int GetPriority(Type type, string method = null);

        /// <summary>
        /// 调试等级
        /// </summary>
        DebugLevels DebugLevel { get; set; }

        /// <summary>
        /// 终止CatLib框架
        /// </summary>
        void Terminate();
    }
}
