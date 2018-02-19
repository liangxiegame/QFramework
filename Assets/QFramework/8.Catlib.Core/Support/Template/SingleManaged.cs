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

using System.Collections.Generic;

namespace CatLib
{
    /// <summary>
    /// 管理器模版（拓展解决方案为单例）- 扩展内容不对外可见
    /// </summary>
    public abstract class SingleManaged<TInterface> : Managed<TInterface> , ISingleManaged<TInterface>
    {
        /// <summary>
        /// 解决方案字典
        /// </summary>
        private readonly Dictionary<string, TInterface> instances = new Dictionary<string, TInterface>();
        
        /// <summary>
        /// 释放解决方案
        /// </summary>
        /// <param name="name">解决方案名</param>
        public void Release(string name = null)
        {
            instances.Remove(StandardName(ref name));
        }

        /// <summary>
        /// 生成解决方案
        /// </summary>
        /// <param name="name">解决方案名</param>
        /// <returns>解决方案实现</returns>
        protected new TInterface MakeExtend(string name)
        {
            StandardName(ref name);
            TInterface element;
            if (instances.TryGetValue(name, out element))
            {
                return element;
            }
            element = base.MakeExtend(name);
            instances.Add(name, element);
            return element;
        }
    }
}
