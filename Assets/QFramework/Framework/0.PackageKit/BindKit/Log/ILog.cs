/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;

namespace Loxodon.Log
{
    /// <summary>
	/// The ILog interface is use by application to log messages into
	/// the Loxodon.Log framework.
	/// </summary>	
	/// <example>Simple example of logging messages
	/// <code lang="C#">
	/// ILog log = LogManager.GetLogger("application-log");
	/// 
	/// log.Info("Application Start");
	/// log.Debug("This is a debug message");
	/// 
	/// if (log.IsDebugEnabled)
	/// {
	///		log.Debug("This is another debug message");
	/// }
	/// </code>
	/// </example>
	/// <author>Clark</author>
	public interface ILog
    {

        void Debug(object message);

        void Debug(object message, Exception exception);

        void DebugFormat(string format, params object[] args);

        void Info(object message);

        void Info(object message, Exception exception);

        void InfoFormat(string format, params object[] args);

        void Warn(object message);

        void Warn(object message, Exception exception);

        void WarnFormat(string format, params object[] args);

        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string format, params object[] args);
        
        void Fatal(object message);

        void Fatal(object message, Exception exception);

        void FatalFormat(string format, params object[] args);

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }
    }
}