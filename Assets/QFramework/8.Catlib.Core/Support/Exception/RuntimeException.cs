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
    /// 运行时异常
    /// </summary>
    public class RuntimeException : Exception
    {
        /// <summary>
        /// 运行时异常
        /// </summary>
        public RuntimeException() : base()
        {
            
        }

        /// <summary>
        /// 运行时异常
        /// </summary>
        /// <param name="message">异常消息</param>
        public RuntimeException(string message) : base(message)
        {
        }

        /// <summary>
        /// 运行时异常
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public RuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}