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

namespace BindKit.Binding.Reflection
{
    public class WeakProxyInvoker : IProxyInvoker
    {
        private WeakReference target;
        private IProxyMethodInfo proxyMethodInfo;
        public WeakProxyInvoker(WeakReference target, IProxyMethodInfo proxyMethodInfo)
        {
            this.target = target;
            this.proxyMethodInfo = proxyMethodInfo;
        }

        public virtual IProxyMethodInfo ProxyMethodInfo { get { return this.proxyMethodInfo; } }

        public object Invoke(params object[] args)
        {
            if (this.proxyMethodInfo.IsStatic)
                return this.proxyMethodInfo.Invoke(null, args);

            if (this.target == null || !this.target.IsAlive)
                return null;

            var obj = this.target.Target;
            if (obj == null)
                return null;

            return this.proxyMethodInfo.Invoke(obj, args);
        }
    }
}
