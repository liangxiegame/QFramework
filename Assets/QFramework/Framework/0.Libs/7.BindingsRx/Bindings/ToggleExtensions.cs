using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class ToggleExtensions
    {
        public static IDisposable BindToggleTo(this Toggle input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.isOn, x => input.isOn = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindToggleTo(this Toggle input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.isOn, x => input.isOn = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}