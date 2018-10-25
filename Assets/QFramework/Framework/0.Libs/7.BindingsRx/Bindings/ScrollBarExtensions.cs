using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class ScrollBarExtensions
    {
        public static IDisposable BindSizeTo(this Scrollbar input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.size, x => input.size = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindSizeTo(this Scrollbar input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.size, x => input.size = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindValueTo(this Scrollbar input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.value, x => input.value = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindValueTo(this Scrollbar input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.value, x => input.value = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindStepsTo(this Scrollbar input, IReactiveProperty<int> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.numberOfSteps, x => input.numberOfSteps = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindStepsTo(this Scrollbar input, Func<int> getter, Action<int> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.numberOfSteps, x => input.numberOfSteps = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}