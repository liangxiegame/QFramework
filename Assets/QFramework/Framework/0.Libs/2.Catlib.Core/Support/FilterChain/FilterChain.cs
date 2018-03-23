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
using System.Collections.Generic;

namespace CatLib
{
    /// <summary>
    /// 过滤器链
    /// </summary>
    /// <typeparam name="TIn">输入参数</typeparam>
    public sealed class FilterChain<TIn> : IFilterChain<TIn>
    {
        /// <summary>
        /// 过滤器链
        /// </summary>
        private readonly SortSet<Action<TIn, Action<TIn>>, int> filterList;

        /// <summary>
        /// 过滤器列表
        /// </summary>
        public Action<TIn, Action<TIn>>[] FilterList
        {
            get
            {
                return filterList.ToArray();
            }
        }

        /// <summary>
        /// 堆栈 用于解决内部递归调用过滤器链所出现的问题
        /// </summary>
        private readonly Stack<int> stack;

        /// <summary>
        /// 构建一个过滤器链
        /// </summary>
        public FilterChain()
        {
            stack = new Stack<int>();
            filterList = new SortSet<Action<TIn, Action<TIn>>, int>();
        }

        /// <summary>
        /// 增加一个过滤器
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="priority">优先级(值越小越优先)</param>
        /// <returns>过滤器链</returns>
        public IFilterChain<TIn> Add(Action<TIn, Action<TIn>> filter, int priority = int.MaxValue)
        {
            filterList.Add(filter, priority);
            return this;
        }

        /// <summary>
        /// 执行过滤器链
        /// </summary>
        /// <param name="inData">输入数据</param>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        public void Do(TIn inData, Action<TIn> then = null)
        {
            if (filterList.Count <= 0)
            {
                if (then != null)
                {
                    then.Invoke(inData);
                }
                return;
            }

            stack.Push(0);
            filterList[0].Invoke(inData, Next(then));
            stack.Pop();
        }

        /// <summary>
        /// 下一层过滤器链
        /// </summary>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        /// <returns>执行过滤器</returns>
        private Action<TIn> Next(Action<TIn> then)
        {
            return (inData) =>
            {
                var index = stack.Pop();
                stack.Push(++index);
                if (index >= filterList.Count)
                {
                    if (then != null)
                    {
                        then.Invoke(inData);
                    }
                    return;
                }
                filterList[index].Invoke(inData, Next(then));
            };
        }
    }

    /// <summary>
    /// 过滤器链
    /// </summary>
    /// <typeparam name="TIn">输入参数</typeparam>
    /// <typeparam name="TOut">输出参数</typeparam>
    public sealed class FilterChain<TIn, TOut> : IFilterChain<TIn, TOut>
    {
        /// <summary>
        /// 过滤器链
        /// </summary>
        private readonly SortSet<Action<TIn, TOut, Action<TIn, TOut>>, int> filterList;

        /// <summary>
        /// 过滤器列表
        /// </summary>
        public Action<TIn, TOut, Action<TIn, TOut>>[] FilterList
        {
            get
            {
                return filterList.ToArray();
            }
        }

        /// <summary>
        /// 堆栈 用于解决内部递归调用过滤器链所出现的问题
        /// </summary>
        private readonly Stack<int> stack;

        /// <summary>
        /// 构建一个过滤器链
        /// </summary>
        public FilterChain()
        {
            stack = new Stack<int>();
            filterList = new SortSet<Action<TIn, TOut, Action<TIn, TOut>>, int>();
        }

        /// <summary>
        /// 增加一个过滤器
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="priority">优先级</param>
        /// <returns>过滤器链</returns>
        public IFilterChain<TIn, TOut> Add(Action<TIn, TOut, Action<TIn, TOut>> filter, int priority = int.MaxValue)
        {
            filterList.Add(filter, priority);
            return this;
        }

        /// <summary>
        /// 执行过滤器链
        /// </summary>
        /// <param name="inData">输入参数</param>
        /// <param name="outData">输出参数</param>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        public void Do(TIn inData, TOut outData, Action<TIn, TOut> then = null)
        {
            if (filterList.Count <= 0)
            {
                if (then != null)
                {
                    then.Invoke(inData, outData);
                }
                return;
            }

            stack.Push(0);
            filterList[0].Invoke(inData, outData, Next(then));
            stack.Pop();
        }

        /// <summary>
        /// 下一层过滤器链
        /// </summary>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        /// <returns>执行过滤器</returns>
        private Action<TIn, TOut> Next(Action<TIn, TOut> then)
        {
            return (inData, outData) =>
            {
                var index = stack.Pop();
                stack.Push(++index);
                if (index >= filterList.Count)
                {
                    if (then != null)
                    {
                        then.Invoke(inData, outData);
                    }
                    return;
                }
                filterList[index].Invoke(inData, outData, Next(then));
            };
        }
    }

    /// <summary>
    /// 过滤器链
    /// </summary>
    /// <typeparam name="TIn">输入参数</typeparam>
    /// <typeparam name="TOut">输出参数</typeparam>
    /// <typeparam name="TException">输入异常</typeparam>
    public sealed class FilterChain<TIn, TOut, TException> : IFilterChain<TIn, TOut, TException>
    {
        /// <summary>
        /// 过滤器链
        /// </summary>
        private readonly SortSet<Action<TIn, TOut, TException, Action<TIn, TOut, TException>>, int> filterList;

        /// <summary>
        /// 过滤器列表
        /// </summary>
        public Action<TIn, TOut, TException, Action<TIn, TOut, TException>>[] FilterList
        {
            get
            {
                return filterList.ToArray();
            }
        }

        /// <summary>
        /// 堆栈 用于解决内部递归调用过滤器链所出现的问题
        /// </summary>
        private readonly Stack<int> stack;

        /// <summary>
        /// 构建一个过滤器链
        /// </summary>
        public FilterChain()
        {
            stack = new Stack<int>();
            filterList = new SortSet<Action<TIn, TOut, TException, Action<TIn, TOut, TException>>, int>();
        }

        /// <summary>
        /// 增加一个过滤器
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="priority">优先级</param>
        /// <returns>过滤器链</returns>
        public IFilterChain<TIn, TOut, TException> Add(Action<TIn, TOut, TException, Action<TIn, TOut, TException>> filter, int priority = int.MaxValue)
        {
            filterList.Add(filter, priority);
            return this;
        }

        /// <summary>
        /// 执行过滤器链
        /// </summary>
        /// <param name="inData">输入参数</param>
        /// <param name="outData">输出参数</param>
        /// <param name="exception">输入异常</param>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        public void Do(TIn inData, TOut outData, TException exception, Action<TIn, TOut, TException> then = null)
        {
            if (filterList.Count <= 0)
            {
                if (then != null)
                {
                    then.Invoke(inData, outData, exception);
                }
                return;
            }

            stack.Push(0);
            filterList[0].Invoke(inData, outData, exception, Next(then));
            stack.Pop();
        }

        /// <summary>
        /// 下一层过滤器链
        /// </summary>
        /// <param name="then">当过滤器执行完成后执行的操作</param>
        /// <returns>执行过滤器</returns>
        private Action<TIn, TOut, TException> Next(Action<TIn, TOut, TException> then)
        {
            return (inData, outData, exception) =>
            {
                var index = stack.Pop();
                stack.Push(++index);
                if (index >= filterList.Count)
                {
                    if (then != null)
                    {
                        then.Invoke(inData, outData, exception);
                    }
                    return;
                }
                filterList[index].Invoke(inData, outData, exception, Next(then));
            };
        }
    }
}
