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

using System.Reflection;
using BindKit.Binding.Binders;
using BindKit.Binding.Converters;
using BindKit.Binding.Paths;
using BindKit.Binding.Proxy.Sources;
using BindKit.Binding.Proxy.Sources.Expressions;
using BindKit.Binding.Proxy.Sources.Object;
using BindKit.Binding.Proxy.Sources.Text;
using BindKit.Binding.Proxy.Targets;
// using BindKit.Services;
using BindKit.Binding.Reflection;
using QFramework;

namespace BindKit.Binding
{
    public class BindingServiceBundle 
    {
        private readonly IQFrameworkContainer mContainer;

        public BindingServiceBundle(IQFrameworkContainer container)
        {
            mContainer = container;
        }

        public void Start()
        {
            OnStart(mContainer);
            
        }

        public void Stop()
        {
            OnStop(mContainer);
        }
        
        protected void OnStart(IQFrameworkContainer container)
        {
            PathParser pathParser = new PathParser();
            ExpressionPathFinder expressionPathFinder = new ExpressionPathFinder();
            ConverterRegistry converterRegistry = new ConverterRegistry();

            ObjectSourceProxyFactory objectSourceProxyFactory = new ObjectSourceProxyFactory();
            objectSourceProxyFactory.Register(new UniversalNodeProxyFactory(), 0);

            SourceProxyFactory sourceFactory = new SourceProxyFactory();
            sourceFactory.Register(new LiteralSourceProxyFactory(), 0);
            sourceFactory.Register(new ExpressionSourceProxyFactory(sourceFactory, expressionPathFinder), 1);
            sourceFactory.Register(objectSourceProxyFactory, 2);

            TargetProxyFactory targetFactory = new TargetProxyFactory();
            targetFactory.Register(new UniversalTargetProxyFactory(), 0);
            targetFactory.Register(new UnityTargetProxyFactory(), 10);
            
            BindingFactory bindingFactory = new BindingFactory(sourceFactory, targetFactory);
            StandardBinder binder = new StandardBinder(bindingFactory);

            container.RegisterInstance<IBinder>(binder);
            container.RegisterInstance<IBindingFactory>(bindingFactory);
            container.RegisterInstance<IConverterRegistry>(converterRegistry);

            container.RegisterInstance<IExpressionPathFinder>(expressionPathFinder);
            container.RegisterInstance<IPathParser>(pathParser);

            container.RegisterInstance<INodeProxyFactory>(objectSourceProxyFactory);
            container.RegisterInstance<INodeProxyFactoryRegister>(objectSourceProxyFactory);

            container.RegisterInstance<ISourceProxyFactory>(sourceFactory);
            container.RegisterInstance<ISourceProxyFactoryRegistry>(sourceFactory);

            container.RegisterInstance<ITargetProxyFactory>(targetFactory);
            container.RegisterInstance<ITargetProxyFactoryRegister>(targetFactory);
        }

        protected void OnStop(IQFrameworkContainer container)
        {
            if (container == null) return;
            container.UnRegisterInstance<IBinder>();
            container.UnRegisterInstance<IBindingFactory>();
            container.UnRegisterInstance<IConverterRegistry>();

            container.UnRegisterInstance<IExpressionPathFinder>();
            container.UnRegisterInstance<IPathParser>();

            container.UnRegisterInstance<INodeProxyFactory>();
            container.UnRegisterInstance<INodeProxyFactoryRegister>();

            container.UnRegisterInstance<ISourceProxyFactory>();
            container.UnRegisterInstance<ISourceProxyFactoryRegistry>();

            container.UnRegisterInstance<ITargetProxyFactory>();
            container.UnRegisterInstance<ITargetProxyFactoryRegister>();
        }
    }
}
