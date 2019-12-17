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

using BindKit.Binding.Contexts;
using BindKit.Binding.Proxy.Sources;
using BindKit.Binding.Proxy.Targets;

namespace BindKit.Binding
{

    public class BindingFactory : IBindingFactory
    {
        private ISourceProxyFactory sourceProxyFactory;
        private ITargetProxyFactory targetProxyFactory;

        public ISourceProxyFactory SourceProxyFactory
        {
            get { return this.sourceProxyFactory; }
            set { this.sourceProxyFactory = value; }
        }
        public ITargetProxyFactory TargetProxyFactory
        {
            get { return this.targetProxyFactory; }
            set { this.targetProxyFactory = value; }
        }

        public BindingFactory(ISourceProxyFactory sourceProxyFactory, ITargetProxyFactory targetProxyFactory)
        {
            this.sourceProxyFactory = sourceProxyFactory;
            this.targetProxyFactory = targetProxyFactory;
        }

        public IBinding Create(IBindingContext bindingContext, object source, object target, BindingDescription bindingDescription)
        {
            return new Binding(bindingContext, source, target, bindingDescription, this.sourceProxyFactory, this.targetProxyFactory);
        }
    }
}
