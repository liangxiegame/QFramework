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
    /// 被绑定对象
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// 当前绑定的名字
        /// </summary>
        string Service { get; }

        /// <summary>
        /// 移除绑定
        /// <para>如果进行的是服务绑定 , 那么在解除绑定时如果是静态化物体将会触发释放</para>
        /// </summary>
        void Unbind();
    }

    /// <summary>
    /// 被绑定对象
    /// </summary>
    public interface IBindable<TReturn> : IBindable
    {
        /// <summary>
        /// 当需求某个服务                                                                                                                                                                                                                                                                                                                                                                                  
        /// </summary>
        /// <param name="service">服务名</param>
        /// <returns>绑定关系临时数据</returns>
        IGivenData<TReturn> Needs(string service);

        /// <summary>
        /// 当需求某个服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>绑定关系临时数据</returns>
        IGivenData<TReturn> Needs<T>();
    }
}
