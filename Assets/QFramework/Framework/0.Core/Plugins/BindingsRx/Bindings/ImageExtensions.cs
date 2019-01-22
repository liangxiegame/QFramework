using System;
using BindingsRx.Converters;
using BindingsRx.Filters;
using UniRx;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class ImageExtensions
    {
        public static IDisposable BindFillAmountTo(this Image input, IReactiveProperty<float> property, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.fillAmount, x => input.fillAmount = x, property, BindingTypes.OneWay, filters).AddTo(input); }
        
        public static IDisposable BindFillAmountTo(this Image input, Func<float> getter, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.fillAmount, x => input.fillAmount = x, getter, null, BindingTypes.OneWay, filters).AddTo(input); }

        public static IDisposable BindFillAmountTo(this Image input, IReactiveProperty<double> property, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.fillAmount, x => input.fillAmount = x, property, new DoubleToFloatConverter(), BindingTypes.OneWay, filters).AddTo(input); }
    }
}