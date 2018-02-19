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
    /// 门面
    /// </summary>
    public abstract class Facade<TService>
    {
        /// <summary>
        /// 实例
        /// </summary>
        private static TService instance;

        /// <summary>
        /// 绑定数据
        /// </summary>
        private static IBindData binder;

        /// <summary>
        /// 是否已经被初始化
        /// </summary>
        private static bool inited;

        /// <summary>
        /// 服务名
        /// </summary>
        private static string service;

        /// <summary>
        /// 是否被释放
        /// </summary>
        private static bool released;

        /// <summary>
        /// 门面静态构造
        /// </summary>
        static Facade()
        {
            service = App.Type2Service(typeof(TService));
            App.OnNewApplication += app =>
            {
                instance = default(TService);
                binder = null;
                inited = false;
                released = false;
                service = App.Type2Service(typeof(TService));
            };
        }

        /// <summary>
        /// 门面实例
        /// </summary>
        public static TService Instance
        {
            get { return Make(); }
        }

        /// <summary>
        /// 是否拥有门面实例
        /// <para>如果为非静态绑定那么永远返回<code>false</code></para>
        /// <para>门面实例判断不能代替:<code>Container.HasInstance</code></para>
        /// </summary>
        internal static bool HasInstance
        {
            get { return binder != null && binder.IsStatic && !released && instance != null; }
        }

        /// <summary>
        /// 构建一个服务实例
        /// </summary>
        /// <param name="userParams">用户参数</param>
        /// <returns>服务实例</returns>
        internal static TService Make(params object[] userParams)
        {
            return HasInstance ? instance : Resolve(userParams);
        }

        /// <summary>
        /// 构建一个服务
        /// </summary>
        private static TService Resolve(params object[] userParams)
        {
            released = false;

            if (!inited)
            {
                App.Watch<TService>(ServiceRebound);
                inited = true;
            }
            else
            {
                // 如果已经初始化了说明binder已经被初始化过。
                // 那么提前判断可以优化性能而不用经过一个hash查找。
                if (binder != null && !binder.IsStatic)
                {
                    return Build(userParams);
                }
            }

            var newBinder = App.GetBind(service);
            if (newBinder == null || !newBinder.IsStatic)
            {
                binder = newBinder;
                return Build(userParams);
            }

            Rebind(newBinder);
            return instance = Build(userParams);
        }

        /// <summary>
        /// 当服务被释放时
        /// </summary>
        /// <param name="oldBinder">旧的绑定器</param>
        /// <param name="_">忽略的参数</param>
        private static void OnRelease(IBindData oldBinder, object _)
        {
            if (oldBinder != binder)
            {
                return;
            }

            instance = default(TService);
            released = true;
        }

        /// <summary>
        /// 当服务被重绑定时
        /// </summary>
        /// <param name="newService">新的服务实例</param>
        private static void ServiceRebound(TService newService)
        {
            var newBinder = App.GetBind(service);
            Rebind(newBinder);
            instance = (newBinder == null || !newBinder.IsStatic) ? default(TService) : newService;
        }

        /// <summary>
        /// 重新绑定
        /// </summary>
        /// <param name="newBinder">新的Binder</param>
        private static void Rebind(IBindData newBinder)
        {
            if (newBinder != null && binder != newBinder && newBinder.IsStatic)
            {
                newBinder.OnRelease(OnRelease);
            }

            binder = newBinder;
        }

        /// <summary>
        /// 生成服务
        /// </summary>
        /// <param name="userParams">服务名</param>
        /// <returns>服务实例</returns>
        private static TService Build(params object[] userParams)
        {
            return (TService)App.Make(service, userParams);
        }
    }
}