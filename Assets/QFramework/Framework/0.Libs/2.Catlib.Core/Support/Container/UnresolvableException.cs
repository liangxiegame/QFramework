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
    /// 未能解决异常
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UnresolvableException : RuntimeException
    {
        /// <summary>
        /// 未能解决异常
        /// </summary>
        public UnresolvableException() : base()
        {

        }

        /// <summary>
        /// 未能解决异常
        /// </summary>
        /// <param name="message">异常消息</param>
        public UnresolvableException(string message) : base(message)
        {
        }

        /// <summary>
        /// 未能解决异常
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public UnresolvableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
