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
    /// 管理器模版 - 不可被外部访问到的拓展
    /// </summary>
    /// <typeparam name="TInterface">拓展接口</typeparam>
    public abstract class Managed<TInterface> : IManaged<TInterface>
    {
        /// <summary>
        /// 自定义解决器
        /// </summary>
        private readonly Dictionary<string, Func<TInterface>> resolve = new Dictionary<string, Func<TInterface>>();

        /// <summary>
        /// 自定义解决方案
        /// </summary>
        /// <param name="resolve">解决方案</param>
        /// <param name="name">名字</param>
        public void Extend(Func<TInterface> resolve, string name = null)
        {
            Guard.Requires<ArgumentNullException>(resolve != null);

            StandardName(ref name);

            if (this.resolve.ContainsKey(name))
            {
                throw new RuntimeException("Extend [" + name + "](" + GetType() + ") is already exists.");
            }

            this.resolve.Add(name, resolve);
        }

        /// <summary>
        /// 释放扩展解决器
        /// </summary>
        /// <param name="name">名字</param>
        public void ReleaseExtend(string name = null)
        {
            StandardName(ref name);
            resolve.Remove(name);
        }

        /// <summary>
        /// 是否包含指定拓展
        /// </summary>
        /// <param name="name">名字</param>
        public bool ContainsExtend(string name = null)
        {
            StandardName(ref name);
            return resolve.ContainsKey(name);
        }

        /// <summary>
        /// 获取解决方案拓展
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>拓展</returns>
        protected Func<TInterface> GetExtend(string name)
        {
            StandardName(ref name);
            Func<TInterface> result;
            if (!resolve.TryGetValue(name, out result))
            {
                throw new RuntimeException("Can not find [" + name + "](" + GetType() + ") Extend.");
            }
            return result;
        }

        /// <summary>
        /// 生成解决的扩展方案
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>解决方案</returns>
        protected TInterface MakeExtend(string name)
        {
            var result = GetExtend(name);
            return result.Invoke();
        }

        /// <summary>
        /// 获取默认名字
        /// </summary>
        /// <returns>默认名字</returns>
        protected virtual string GetDefaultName()
        {
            return "default";
        }

        /// <summary>
        /// 标准化名字
        /// </summary>
        /// <param name="name">名字</param>
        protected string StandardName(ref string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = GetDefaultName();
            }
            return name;
        }
    }
}
