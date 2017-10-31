/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;

    /// <summary>
    /// 条件判断是否符合条件
    /// </summary>
    public interface ICondition
    {
        bool IsSatisfy();
    }

    /// <summary>
    /// 抽象类
    /// </summary>
    public abstract class Condition : ICondition, IDisposable
    {
        protected Func<bool> mConditionFunc;

        public bool IsSatisfy()
        {
            return mConditionFunc.InvokeGracefully();
        }

        public void Dispose()
        {
            mConditionFunc = null;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }

    /// <summary>
    /// 默认的条件
    /// </summary>
    public class DefaultCondition : Condition
    {
        public DefaultCondition(Func<bool> condition)
        {
            mConditionFunc = condition;
        }
    }
}