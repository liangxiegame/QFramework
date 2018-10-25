using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;

namespace BindingsRx.Bindings
{
    public static class MonoBehaviourExtensions
    {
        public static IDisposable BindEnabledTo(this MonoBehaviour input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.enabled, x => input.enabled = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindEnabledTo(this MonoBehaviour input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.enabled, x => input.enabled = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}