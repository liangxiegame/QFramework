/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;

namespace CatLib
{
    /// <summary>
    /// 过滤器链
    /// </summary>
    /// <typeparam name="TIn">输入参数</typeparam>
    public interface IFilterChain<TIn>
    {
        /// <summary>
        /// 过滤器列表
        /// </summary>
        Action<TIn, Action<TIn>>[] FilterList { get; }

        /// <summary>
        /// 增加过滤器
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="priority">优先级(值越小越优先)</param>
        /// <returns>过滤器链</returns>
        IFilterChain<TIn> Add(Action<TIn, Action<TIn>> filter, int priority = int.MaxValue);

        /// <summary>
        /// 执行过滤器链
        /// </summary>
        /// <param name="inData">输入数据</param>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        void Do(TIn inData, Action<TIn> then = null);
    }

    /// <summary>
    /// 过滤器链
    /// </summary>
    /// <typeparam name="TIn">输入参数</typeparam>
    /// <typeparam name="TOut">输出参数</typeparam>
    public interface IFilterChain<TIn, TOut>
    {
        /// <summary>
        /// 过滤器列表
        /// </summary>
        Action<TIn, TOut, Action<TIn, TOut>>[] FilterList { get; }

        /// <summary>
        /// 增加一个过滤器
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="priority">优先级(值越小越优先)</param>
        /// <returns>过滤器链</returns>
        IFilterChain<TIn, TOut> Add(Action<TIn, TOut, Action<TIn, TOut>> filter, int priority = int.MaxValue);

        /// <summary>
        /// 执行过滤器链
        /// </summary>
        /// <param name="inData">输入参数</param>
        /// <param name="outData">输出参数</param>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        void Do(TIn inData, TOut outData, Action<TIn, TOut> then = null);
    }

    /// <summary>
    /// 过滤器链
    /// </summary>
    /// <typeparam name="TIn">输入参数</typeparam>
    /// <typeparam name="TOut">输出参数</typeparam>
    /// <typeparam name="TException">输入异常</typeparam>
    public interface IFilterChain<TIn, TOut, TException>
    {
        /// <summary>
        /// 过滤器列表
        /// </summary>
        Action<TIn, TOut, TException, Action<TIn, TOut, TException>>[] FilterList { get; }

        /// <summary>
        /// 增加一个过滤器
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="priority">优先级(值越小越优先)</param>
        /// <returns>过滤器链</returns>
        IFilterChain<TIn, TOut, TException> Add(Action<TIn, TOut, TException, Action<TIn, TOut, TException>> filter, int priority = int.MaxValue);

        /// <summary>
        /// 执行过滤器链
        /// </summary>
        /// <param name="inData">输入参数</param>
        /// <param name="outData">输出参数</param>
        /// <param name="exception">输入异常</param>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        void Do(TIn inData, TOut outData, TException exception, Action<TIn, TOut, TException> then = null);
    }
}