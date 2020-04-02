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
using System.Collections.Generic;
using BindKit.Binding.Paths;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class ObjectSourceProxyFactory : TypedSourceProxyFactory<ObjectSourceDescription>, INodeProxyFactory, INodeProxyFactoryRegister
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ObjectSourceProxyFactory));

        private List<PriorityFactoryPair> factories = new List<PriorityFactoryPair>();

        protected override bool TryCreateProxy(object source, ObjectSourceDescription description, out ISourceProxy proxy)
        {
            proxy = null;
            var path = description.Path;
            if (path.Count <= 0)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Unable to bind: an empty path node list!.");
                return false;
            }

            PathToken token = path.AsPathToken();

            if (path.Count == 1)
            {
                proxy = this.Create(source, token);
                if (proxy != null)
                    return true;
                return false;
            }

            proxy = new ChainedObjectSourceProxy(source, token, this);
            return true;
        }

        public virtual ISourceProxy Create(object source, PathToken token)
        {
            ISourceProxy proxy = null;
            foreach (PriorityFactoryPair pair in this.factories)
            {
                var factory = pair.factory;
                if (factory == null)
                    continue;

                proxy = factory.Create(source, token);
                if (proxy != null)
                    return proxy;
            }
            return proxy;
        }

        public virtual void Register(INodeProxyFactory factory, int priority = 100)
        {
            if (factory == null)
                return;

            this.factories.Add(new PriorityFactoryPair(factory, priority));
            this.factories.Sort((x, y) => y.priority.CompareTo(x.priority));
        }

        public virtual void Unregister(INodeProxyFactory factory)
        {
            if (factory == null)
                return;

            this.factories.RemoveAll(pair => pair.factory == factory);
        }

        struct PriorityFactoryPair
        {
            public PriorityFactoryPair(INodeProxyFactory factory, int priority)
            {
                this.factory = factory;
                this.priority = priority;
            }
            public int priority;
            public INodeProxyFactory factory;
        }
    }
}
