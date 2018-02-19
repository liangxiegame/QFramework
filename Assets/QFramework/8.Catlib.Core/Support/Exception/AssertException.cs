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
    /// 断言异常
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AssertException : RuntimeException
    {
        /// <summary>
        /// 断言异常
        /// </summary>
        public AssertException() : base()
        {
        }

        /// <summary>
        /// 断言异常
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string message) : base(message)
        {
        }

        /// <summary>
        /// 断言异常
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public AssertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
