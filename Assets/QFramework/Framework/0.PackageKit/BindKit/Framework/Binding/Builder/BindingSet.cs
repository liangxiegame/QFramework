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
using BindKit.Binding.Contexts;
using Loxodon.Log;

namespace BindKit.Binding.Builder
{
    public abstract class BindingSetBase : IBindingBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BindingSetBase));

        protected IBindingContext context;
        protected readonly List<IBindingBuilder> builders = new List<IBindingBuilder>();

        public BindingSetBase(IBindingContext context)
        {
            this.context = context;
        }

        public virtual void Build()
        {
            foreach (var builder in this.builders)
            {
                try
                {
                    builder.Build();
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled)
                        log.ErrorFormat("{0}", e);
                }
            }
            this.builders.Clear();
        }
    }

    public class BindingSet<TTarget, TSource> : BindingSetBase where TTarget : class
    {
        private TTarget target;
        public   BindingSet(IBindingContext context, TTarget target) : base(context)
        {
            this.target = target;
        }

        public virtual BindingBuilder<TTarget, TSource> Bind()
        {
            var builder = new BindingBuilder<TTarget, TSource>(this.context, this.target);
            this.builders.Add(builder);
            return builder;
        }

        public virtual BindingBuilder<TChildTarget, TSource> Bind<TChildTarget>(TChildTarget target) where TChildTarget : class
        {
            var builder = new BindingBuilder<TChildTarget, TSource>(context, target);
            this.builders.Add(builder);
            return builder;
        }
    }

    public class BindingSet<TTarget> : BindingSetBase where TTarget : class
    {
        private TTarget target;
        public BindingSet(IBindingContext context, TTarget target) : base(context)
        {
            this.target = target;
        }

        public virtual BindingBuilder<TTarget> Bind()
        {
            var builder = new BindingBuilder<TTarget>(this.context, this.target);
            this.builders.Add(builder);
            return builder;
        }

        public virtual BindingBuilder<TChildTarget> Bind<TChildTarget>(TChildTarget target) where TChildTarget : class
        {
            var builder = new BindingBuilder<TChildTarget>(context, target);
            this.builders.Add(builder);
            return builder;
        }
    }

    public class BindingSet : BindingSetBase
    {
        private object target;
        public BindingSet(IBindingContext context, object target) : base(context)
        {
            this.target = target;
        }

        public virtual BindingBuilder Bind()
        {
            var builder = new BindingBuilder(this.context, this.target);
            this.builders.Add(builder);
            return builder;
        }

        public virtual BindingBuilder Bind(object target)
        {
            var builder = new BindingBuilder(context, target);
            this.builders.Add(builder);
            return builder;
        }
    }
}
