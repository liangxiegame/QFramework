using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class ScrollRectExtensions
    {
        public static IDisposable BindDecelerationRateTo(this ScrollRect input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.decelerationRate, x => input.decelerationRate = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindDecelerationRateTo(this ScrollRect input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.decelerationRate, x => input.decelerationRate = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindElasticityTo(this ScrollRect input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.elasticity, x => input.elasticity = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindElasticityTo(this ScrollRect input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.elasticity, x => input.elasticity = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindHorizontalTo(this ScrollRect input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.horizontal, x => input.horizontal = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindHorizontalTo(this ScrollRect input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.horizontal, x => input.horizontal = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindVerticalTo(this ScrollRect input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.vertical, x => input.vertical = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindVerticalTo(this ScrollRect input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.vertical, x => input.vertical = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}