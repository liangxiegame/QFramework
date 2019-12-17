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
using System.Collections.Generic;

namespace Loxodon.Log
{
    public class DefaultLogFactory : ILogFactory
    {
        private Dictionary<string, ILog> repositories = new Dictionary<string, ILog>();
        private bool inUnity = true;
        private Level _level = Level.ALL;

        public DefaultLogFactory()
        {
        }

        public Level Level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        public bool InUnity
        {
            get { return this.inUnity; }
            set { this.inUnity = value; }
        }

        public ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public ILog GetLogger(Type type)
        {
            ILog log;
            if (repositories.TryGetValue(type.FullName, out log))
                return log;

            log = new LogImpl(type.Name, this);
            repositories[type.FullName] = log;
            return log;
        }

        public ILog GetLogger(string name)
        {
            ILog log;
            if (repositories.TryGetValue(name, out log))
                return log;

            log = new LogImpl(name, this);
            repositories[name] = log;
            return log;
        }
    }

    internal class LogImpl : ILog
    {
        private string name;
        private DefaultLogFactory _factory;
        public LogImpl(string name, DefaultLogFactory factory)
        {
            this.name = name;
            this._factory = factory;
        }

        public string Name { get { return this.name; } }

        protected virtual string Format(object message, string level)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2} - {3}", System.DateTime.Now, level, name, message);
        }

        public virtual void Debug(object message)
        {
            if (this._factory.InUnity)
                UnityEngine.Debug.Log(Format(message, "DEBUG"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "DEBUG"));
#endif
        }

        public virtual void Debug(object message, Exception exception)
        {
            Debug(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void DebugFormat(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public virtual void Info(object message)
        {
            if (this._factory.InUnity)
                UnityEngine.Debug.Log(Format(message, "INFO"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "INFO"));
#endif
        }

        public virtual void Info(object message, Exception exception)
        {
            Info(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void InfoFormat(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public virtual void Warn(object message)
        {
            if (this._factory.InUnity)
                UnityEngine.Debug.LogWarning(Format(message, "WARN"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "WARN"));
#endif
        }

        public virtual void Warn(object message, Exception exception)
        {
            Warn(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void WarnFormat(string format, params object[] args)
        {
            Warn(string.Format(format, args));
        }

        public virtual void Error(object message)
        {
            if (this._factory.InUnity)
                UnityEngine.Debug.LogError(Format(message, "ERROR"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "ERROR"));
#endif
        }

        public virtual void Error(object message, Exception exception)
        {
            Error(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void ErrorFormat(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public virtual void Fatal(object message)
        {
            if (this._factory.InUnity)
                UnityEngine.Debug.LogError(Format(message, "FATAL"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "FATAL"));
#endif
        }

        public virtual void Fatal(object message, Exception exception)
        {
            Fatal(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void FatalFormat(string format, params object[] args)
        {
            Fatal(string.Format(format, args));
        }

        protected bool IsEnabled(Level level)
        {
            return level >= this._factory.Level;
        }

        public virtual bool IsDebugEnabled
        {
            get { return IsEnabled(Level.DEBUG); }
        }

        public virtual bool IsInfoEnabled
        {
            get { return IsEnabled(Level.INFO); }
        }

        public virtual bool IsWarnEnabled
        {
            get { return IsEnabled(Level.WARN); }
        }

        public virtual bool IsErrorEnabled
        {
            get { return IsEnabled(Level.ERROR); }
        }

        public virtual bool IsFatalEnabled
        {
            get { return IsEnabled(Level.FATAL); }
        }
    }
}
