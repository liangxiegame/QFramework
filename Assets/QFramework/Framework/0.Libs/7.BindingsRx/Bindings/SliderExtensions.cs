using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class SliderExtensions
    {
        public static IDisposable BindValueTo(this Slider input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.value, x => input.value = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindValueTo(this Slider input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.value, x => input.value = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindMaxValueTo(this Slider input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.maxValue, x => input.maxValue = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindMaxValueTo(this Slider input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.maxValue, x => input.maxValue = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindMinValueTo(this Slider input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.minValue, x => input.minValue = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindMinValueTo(this Slider input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.minValue, x => input.minValue = x, getter, setter, bindingType, filters).AddTo(input); }
        
        public static IDisposable BindNormalizedValueTo(this Slider input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.normalizedValue, x => input.normalizedValue = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindNormalizedValueTo(this Slider input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.normalizedValue, x => input.normalizedValue = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}