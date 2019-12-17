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

using Loxodon.Log;
using System;
using BindKit.Binding.Paths;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class ChainedObjectSourceProxy : NotifiableSourceProxyBase, IObtainable, IModifiable, INotifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ChainedObjectSourceProxy));

        private INodeProxyFactory factory;
        private ProxyEntry[] proxies;
        private int count;

        public ChainedObjectSourceProxy(object source, PathToken token, INodeProxyFactory factory) : base(source)
        {
            this.factory = factory;
            count = token.Path.Count;
            proxies = new ProxyEntry[count];
            Bind(source, token);
        }

        public override Type Type
        {
            get
            {
                var proxy = GetProxy();
                if (proxy == null)
                    return typeof(object);

                return proxy.Type;
            }
        }

        protected ISourceProxy GetProxy()
        {
            ProxyEntry proxyEntry = proxies[count - 1];
            if (proxyEntry == null)
                return null;

            return proxyEntry.Proxy;
        }

        protected IObtainable GetObtainable()
        {
            ProxyEntry proxyEntry = proxies[count - 1];
            if (proxyEntry == null)
                return null;

            return proxyEntry.Proxy as IObtainable;
        }

        protected IModifiable GetModifiable()
        {
            ProxyEntry proxyEntry = proxies[count - 1];
            if (proxyEntry == null)
                return null;

            return proxyEntry.Proxy as IModifiable;
        }

        public virtual object GetValue()
        {
            IObtainable obtainable = this.GetObtainable();
            if (obtainable == null)
                return null;
            return obtainable.GetValue();
        }

        public virtual TValue GetValue<TValue>()
        {
            IObtainable obtainable = this.GetObtainable();
            if (obtainable == null)
                return default(TValue);

            return obtainable.GetValue<TValue>();
        }

        public virtual void SetValue(object value)
        {
            IModifiable modifiable = this.GetModifiable();
            if (modifiable == null)
                return;

            modifiable.SetValue(value);
        }

        public virtual void SetValue<TValue>(TValue value)
        {
            IModifiable modifiable = this.GetModifiable();
            if (modifiable == null)
                return;

            modifiable.SetValue<TValue>(value);
        }

        void Bind(object source, PathToken token)
        {
            int index = token.Index;
            ISourceProxy proxy = factory.Create(source, token);
            if (proxy == null)
            {
                var node = token.Current;
                if (node is MemberNode)
                {
                    var memberNode = node as MemberNode;
                    string typeName = source != null ? source.GetType().Name : memberNode.Type.Name;
                    throw new Exception(string.Format("Not found the member named '{0}' in the '{1}' type", memberNode.Name, typeName));
                }
                throw new Exception("proxy is null.");
            }

            ProxyEntry entry = new ProxyEntry(proxy, token);
            proxies[index] = entry;

            if (token.HasNext())
            {
                if (proxy is INotifiable)
                {
                    entry.Handler = (sender, args) =>
                    {
                        lock (_lock)
                        {
                            try
                            {
                                var proxyEntry = proxies[index];
                                if (proxyEntry == null || sender != proxyEntry.Proxy)
                                    return;

                                Rebind(index);
                            }
                            catch (Exception e)
                            {
                                if (log.IsErrorEnabled)
                                    log.ErrorFormat("{0}", e);
                            }
                        }
                    };
                }

                var child = (proxy as IObtainable).GetValue();
                if (child != null)
                    Bind(child, token.NextToken());
                else
                    this.RaiseValueChanged();
            }
            else
            {
                if (proxy is INotifiable)
                    entry.Handler = (sender, args) => { this.RaiseValueChanged(); };
                this.RaiseValueChanged();
            }
        }

        void Rebind(int index)
        {
            for (int i = proxies.Length - 1; i > index; i--)
            {
                ProxyEntry proxyEntry = proxies[i];
                if (proxyEntry == null)
                    continue;

                var proxy = proxyEntry.Proxy;
                proxyEntry.Proxy = null;
                if (proxy != null)
                    proxy.Dispose();
            }

            ProxyEntry entry = proxies[index];
            var obtainable = entry.Proxy as IObtainable;
            if (obtainable == null)
            {
                this.RaiseValueChanged();
                return;
            }

            var source = obtainable.GetValue();
            if (source == null)
            {
                this.RaiseValueChanged();
                return;
            }

            Bind(source, entry.Token.NextToken());
        }

        void Unbind()
        {
            for (int i = proxies.Length - 1; i <= 0; i--)
            {
                ProxyEntry proxyEntry = proxies[i];
                if (proxyEntry == null)
                    continue;

                proxyEntry.Dispose();
                proxies[i] = null;
            }
        }

        #region IDisposable Support    
        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Unbind();
                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion

        public class ProxyEntry : IDisposable
        {
            private ISourceProxy proxy;
            private EventHandler handler;
            public ProxyEntry(ISourceProxy proxy, PathToken token)
            {
                this.Proxy = proxy;
                this.Token = token;
            }

            public ISourceProxy Proxy
            {
                get { return this.proxy; }
                set
                {
                    if (this.proxy == value)
                        return;

                    if (this.handler != null)
                    {
                        var notifiable = this.proxy as INotifiable;
                        if (notifiable != null)
                            notifiable.ValueChanged -= this.handler;

                        notifiable = value as INotifiable;
                        if (notifiable != null)
                            notifiable.ValueChanged += this.handler;
                    }

                    this.proxy = value;
                }
            }

            public PathToken Token { get; set; }

            public EventHandler Handler
            {
                get { return this.handler; }
                set
                {
                    if (this.handler == value)
                        return;

                    var notifiable = this.proxy as INotifiable;
                    if (notifiable != null)
                    {
                        if (this.handler != null)
                            notifiable.ValueChanged -= this.handler;

                        if (value != null)
                            notifiable.ValueChanged += value;
                    }

                    this.handler = value;
                }
            }

            #region IDisposable Support
            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    this.Handler = null;
                    if (this.proxy != null)
                        this.proxy.Dispose();
                    this.proxy = null;
                    this.Token = null;
                    disposedValue = true;
                }
            }

            ~ProxyEntry()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }

    }
}
