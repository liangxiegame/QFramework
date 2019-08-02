using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;

namespace BindingsRx.Bindings
{
    public static class GameObjectExtensions
    {
        public static IDisposable BindActiveTo(this GameObject input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.activeSelf, input.SetActive, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindActiveTo(this GameObject input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.activeSelf, input.SetActive, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindLayerTo(this GameObject input, IReactiveProperty<int> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.layer, x => input.layer = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindLayerTo(this GameObject input, Func<int> getter, Action<int> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.layer, x => input.layer = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindLayerTo(this GameObject input, IReactiveProperty<LayerMask> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<LayerMask>[] filters)
        { return GenericBindings.Bind(() => input.layer, x => input.layer = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindLayerTo(this GameObject input, Func<LayerMask> getter, Action<LayerMask> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<LayerMask>[] filters)
        { return GenericBindings.Bind(() => input.layer, x => input.layer = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindNameTo(this GameObject input, IReactiveProperty<string> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        { return GenericBindings.Bind(() => input.name, x => input.name = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindNameTo(this GameObject input, Func<string> getter, Action<string> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        { return GenericBindings.Bind(() => input.name, x => input.name = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindTagTo(this GameObject input, IReactiveProperty<string> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        { return GenericBindings.Bind(() => input.tag, x => input.tag = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindTagTo(this GameObject input, Func<string> getter, Action<string> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        { return GenericBindings.Bind(() => input.tag, x => input.tag = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}