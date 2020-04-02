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
using System.Linq.Expressions;
using BindKit.Binding.Contexts;
using BindKit.Binding.Converters;
using UnityEngine.Events;

using Loxodon.Log;
// using BindKit.Interactivity;

namespace BindKit.Binding.Builder
{
    public class BindingBuilder<TTarget, TSource> : BindingBuilderBase where TTarget : class
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(BindingBuilder<TTarget, TSource>));

        public BindingBuilder(IBindingContext context, TTarget target) : base(context, target)
        {
        }

        public BindingBuilder<TTarget, TSource> For(string targetName)
        {
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = null;
            return this;
        }

        public BindingBuilder<TTarget, TSource> For(string targetName, string updateTrigger)
        {
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        public BindingBuilder<TTarget, TSource> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = null;
            return this;
        }

        public BindingBuilder<TTarget, TSource> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression, string updateTrigger)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        public BindingBuilder<TTarget, TSource> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression, Expression<Func<TTarget, UnityEventBase>> updateTriggerExpression)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            string updateTrigger = this.PathParser.ParseMemberName(updateTriggerExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        [Obsolete("Please use \"For(v => v.MethodName)\" instead of this method.")]
        public BindingBuilder<TTarget, TSource> For(Expression<Action<TTarget>> memberExpression)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = null;
            this.OneWayToSource();
            return this;
        }

        // public BindingBuilder<TTarget, TSource> For(Expression<Func<TTarget, EventHandler<InteractionEventArgs>>> memberExpression)
        // {
        //     string targetName = this.PathParser.ParseMemberName(memberExpression);
        //     this.description.TargetName = targetName;
        //     this.description.UpdateTrigger = null;
        //     this.OneWayToSource();
        //     return this;
        // }

        public BindingBuilder<TTarget, TSource> To(string path)
        {
            this.SetMemberPath(path);
            return this;
        }

        public BindingBuilder<TTarget, TSource> To<TResult>(Expression<Func<TSource, TResult>> path)
        {
            this.SetMemberPath(this.PathParser.Parse(path));
            return this;
        }

        [Obsolete("Please use \"To(vm => vm.MethodName)\" or \"To<TParameter>(vm => vm.MethodName)\" instead of this method.")]
        public BindingBuilder<TTarget, TSource> To(Expression<Action<TSource>> path)
        {
            this.SetMemberPath(this.PathParser.Parse(path));
            return this;
        }

        public BindingBuilder<TTarget, TSource> To<TParameter>(Expression<Func<TSource, Action<TParameter>>> path)
        {
            this.SetMemberPath(this.PathParser.Parse(path));
            return this;
        }

        public BindingBuilder<TTarget, TSource> To(Expression<Func<TSource, Action>> path)
        {
            this.SetMemberPath(this.PathParser.Parse(path));
            return this;
        }

        public BindingBuilder<TTarget, TSource> ToExpression<TResult>(Expression<Func<TSource, TResult>> expression)
        {
            this.SetExpression(expression);
            this.OneWay();
            return this;
        }

        public BindingBuilder<TTarget, TSource> TwoWay()
        {
            this.SetMode(BindingMode.TwoWay);
            return this;
        }

        public BindingBuilder<TTarget, TSource> OneWay()
        {
            this.SetMode(BindingMode.OneWay);
            return this;
        }

        public BindingBuilder<TTarget, TSource> OneWayToSource()
        {
            this.SetMode(BindingMode.OneWayToSource);
            return this;
        }

        public BindingBuilder<TTarget, TSource> OneTime()
        {
            this.SetMode(BindingMode.OneTime);
            return this;
        }

        public BindingBuilder<TTarget, TSource> CommandParameter(object parameter)
        {
            this.SetCommandParameter(parameter);
            return this;
        }

        public BindingBuilder<TTarget, TSource> WithConversion(string converterName)
        {
            var converter = this.ConverterByName(converterName);
            return this.WithConversion(converter);
        }

        public BindingBuilder<TTarget, TSource> WithConversion(IConverter converter)
        {
            this.description.Converter = converter;
            return this;
        }

        public BindingBuilder<TTarget, TSource> WithScopeKey(object scopeKey)
        {
            this.SetScopeKey(scopeKey);
            return this;
        }
    }

    public class BindingBuilder<TTarget> : BindingBuilderBase where TTarget : class
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(BindingBuilder<TTarget>));

        public BindingBuilder(IBindingContext context, TTarget target) : base(context, target)
        {
        }

        public BindingBuilder<TTarget> For(string targetPropertyName)
        {
            this.description.TargetName = targetPropertyName;
            this.description.UpdateTrigger = null;
            return this;
        }

        public BindingBuilder<TTarget> For(string targetPropertyName, string updateTrigger)
        {
            this.description.TargetName = targetPropertyName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = null;
            return this;
        }

        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression, string updateTrigger)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression, Expression<Func<TTarget, UnityEventBase>> updateTriggerExpression)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            string updateTrigger = this.PathParser.ParseMemberName(updateTriggerExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        [Obsolete("Please use \"For(v => v.MethodName)\" instead of this method.")]
        public BindingBuilder<TTarget> For(Expression<Action<TTarget>> memberExpression)
        {
            string targetName = this.PathParser.ParseMemberName(memberExpression);
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = null;
            this.OneWayToSource();
            return this;
        }

        // public BindingBuilder<TTarget> For(Expression<Func<TTarget, EventHandler<InteractionEventArgs>>> memberExpression)
        // {
        //     string targetName = this.PathParser.ParseMemberName(memberExpression);
        //     this.description.TargetName = targetName;
        //     this.description.UpdateTrigger = null;
        //     this.OneWayToSource();
        //     return this;
        // }

        public BindingBuilder<TTarget> To(string path)
        {
            this.SetStaticMemberPath(path);
            this.OneWay();
            return this;
        }

        public BindingBuilder<TTarget> To<TResult>(Expression<Func<TResult>> path)
        {
            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
            this.OneWay();
            return this;
        }

        [Obsolete("Please use \"To(() => Class.MethodName)\" or \"To<TParameter>(() => Class.MethodName)\" instead of this method.")]
        public BindingBuilder<TTarget> To(Expression<Action> path)
        {
            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
            return this;
        }

        public BindingBuilder<TTarget> To<TParameter>(Expression<Func<Action<TParameter>>> path)
        {
            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
            return this;
        }

        public BindingBuilder<TTarget> To(Expression<Func<Action>> path)
        {
            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
            return this;
        }

        public BindingBuilder<TTarget> ToValue(object value)
        {
            this.SetLiteral(value);
            return this;
        }

        public BindingBuilder<TTarget> ToExpression<TResult>(Expression<Func<TResult>> expression)
        {
            this.SetExpression(expression);
            this.OneWay();
            return this;
        }

        public BindingBuilder<TTarget> TwoWay()
        {
            this.SetMode(BindingMode.TwoWay);
            return this;
        }

        public BindingBuilder<TTarget> OneWay()
        {
            this.SetMode(BindingMode.OneWay);
            return this;
        }

        public BindingBuilder<TTarget> OneWayToSource()
        {
            this.SetMode(BindingMode.OneWayToSource);
            return this;
        }

        public BindingBuilder<TTarget> OneTime()
        {
            this.SetMode(BindingMode.OneTime);
            return this;
        }

        public BindingBuilder<TTarget> CommandParameter(object parameter)
        {
            this.SetCommandParameter(parameter);
            return this;
        }

        public BindingBuilder<TTarget> WithConversion(string converterName)
        {
            var converter = ConverterByName(converterName);
            return this.WithConversion(converter);
        }

        public BindingBuilder<TTarget> WithConversion(IConverter converter)
        {
            this.description.Converter = converter;
            return this;
        }

        public BindingBuilder<TTarget> WithScopeKey(object scopeKey)
        {
            this.SetScopeKey(scopeKey);
            return this;
        }
    }

    public class BindingBuilder : BindingBuilderBase
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(BindingBuilder));

        public BindingBuilder(IBindingContext context, object target) : base(context, target)
        {
        }

        public BindingBuilder For(string targetName, string updateTrigger = null)
        {
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        public BindingBuilder To(string path)
        {
            this.SetMemberPath(path);
            return this;
        }

        public BindingBuilder ToStatic(string path)
        {
            this.SetStaticMemberPath(path);
            return this;
        }

        public BindingBuilder ToValue(object value)
        {
            this.SetLiteral(value);
            return this;
        }

        public BindingBuilder TwoWay()
        {
            this.SetMode(BindingMode.TwoWay);
            return this;
        }

        public BindingBuilder OneWay()
        {
            this.SetMode(BindingMode.OneWay);
            return this;
        }

        public BindingBuilder OneWayToSource()
        {
            this.SetMode(BindingMode.OneWayToSource);
            return this;
        }

        public BindingBuilder OneTime()
        {
            this.SetMode(BindingMode.OneTime);
            return this;
        }

        public BindingBuilder WithConversion(string converterName)
        {
            var converter = this.ConverterByName(converterName);
            return this.WithConversion(converter);
        }

        public BindingBuilder WithConversion(IConverter converter)
        {
            this.description.Converter = converter;
            return this;
        }

        public BindingBuilder WithScopeKey(object scopeKey)
        {
            this.SetScopeKey(scopeKey);
            return this;
        }
    }

}
