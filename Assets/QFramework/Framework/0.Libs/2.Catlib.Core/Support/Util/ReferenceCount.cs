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
using System.Threading;

namespace CatLib
{
    /// <summary>
    /// 引用计数
    /// </summary>
    public class ReferenceCount : IReferenceCount
    {
        /// <summary>
        /// 计数
        /// </summary>
        private int count;

        /// <summary>
        /// 释放句柄
        /// </summary>
        private readonly IDisposable disposable;

        /// <summary>
        /// 当前计数
        /// </summary>
        public int RefCount
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public ReferenceCount()
        {
            disposable = this as IDisposable;
            Guard.Requires<InvalidCastException>(disposable != null);
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        /// <param name="disposable">释放接口</param>
        public ReferenceCount(IDisposable disposable)
        {
            Guard.Requires<ArgumentNullException>(disposable != null);
            this.disposable = disposable;
        }

        /// <summary>
        /// 加计数。
        /// </summary>
        public virtual void Retain()
        {
            var value = Interlocked.Increment(ref count);
            Guard.Requires<AssertException>(value > 0);
        }

        /// <summary>
        /// 减计数
        /// </summary>
        public virtual void Release()
        {
            if (Interlocked.Decrement(ref count) == 0)
            {
                disposable.Dispose();
            }
            Guard.Requires<AssertException>(count >= 0);
        }
    }
}
