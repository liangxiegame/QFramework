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

namespace CatLib
{
    /// <summary>
    /// 绑定关系临时数据,用于支持链式调用
    /// </summary>
    internal sealed class GivenData<TReturn> : IGivenData<TReturn> where TReturn : class, IBindable<TReturn>
    {
        /// <summary>
        /// 绑定数据
        /// </summary>
        private readonly Bindable<TReturn> bindable;

        /// <summary>
        /// 父级容器
        /// </summary>
        private readonly Container container;

        /// <summary>
        /// 需求什么服务
        /// </summary>
        private string needs;

        /// <summary>
        /// 绑定关系临时数据
        /// </summary>
        /// <param name="container">依赖注入容器</param>
        /// <param name="bindable">可绑定数据</param>
        internal GivenData(Container container, Bindable<TReturn> bindable)
        {
            this.container = container;
            this.bindable = bindable;
        }

        /// <summary>
        /// 需求什么服务
        /// </summary>
        /// <param name="needs">需求什么服务</param>
        /// <returns>绑定关系实例</returns>
        internal IGivenData<TReturn> Needs(string needs)
        {
            this.needs = needs;
            return this;
        }

        /// <summary>
        /// 给与什么服务
        /// </summary>
        /// <param name="service">给与的服务名或别名</param>
        /// <returns>服务绑定数据</returns>
        public TReturn Given(string service)
        {
            Guard.NotEmptyOrNull(service, "service");
            bindable.AddContextual(needs, service);
            return bindable as TReturn;
        }

        /// <summary>
        /// 给与什么服务
        /// </summary>
        /// <typeparam name="T">给与的服务名或别名</typeparam>
        /// <returns>服务绑定数据</returns>
        public TReturn Given<T>()
        {
            return Given(container.Type2Service(typeof(T)));
        }
    }
}