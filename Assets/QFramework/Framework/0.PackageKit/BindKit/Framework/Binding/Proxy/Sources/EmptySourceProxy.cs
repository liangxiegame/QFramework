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
using Loxodon.Log;

namespace BindKit.Binding.Proxy.Sources
{
    public class EmptSourceProxy : SourceProxyBase, IObtainable, IModifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmptSourceProxy));

        private SourceDescription description;

        public EmptSourceProxy(SourceDescription description) : base(null)
        {
            this.description = description;
        }

        public override Type Type { get { return typeof(object); } }

        public virtual object GetValue()
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());

            return null;
        }

        public virtual TValue GetValue<TValue>()
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());

            return default(TValue);
        }

        public virtual void SetValue(object value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());
        }

        public virtual void SetValue<TValue>(TValue value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());
        }
    }
}
