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
    /// 应用程序事件
    /// </summary>
    public sealed class ApplicationEvents
    {
        /// <summary>
        /// 当引导完成时
        /// </summary>
        public static readonly string OnBootstraped = "CatLib.ApplicationEvents.OnBootstraped";

        /// <summary>
        /// 当初始化进行时
        /// </summary>
        public static readonly string OnIniting = "CatLib.ApplicationEvents.OnIniting";

        /// <summary>
        /// 当程序启动完成
        /// </summary>
        public static readonly string OnStartCompleted = "CatLib.ApplicationEvents.OnStartCompleted";

        /// <summary>
        /// 当程序终止之前
        /// </summary>
        public static readonly string OnTerminate = "CatLib.ApplicationEvents.OnTerminate";

        /// <summary>
        /// 当程序终止之后
        /// </summary>
        public static readonly string OnTerminated = "CatLib.ApplicationEvents.OnTerminated";
    }
}