using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;

namespace BindingsRx.Bindings
{
    /// <summary>
    /// All material bindings require explicit cleanup as Material is not IDisposable
    /// </summary>
    public static class MaterialExtensions
    {
        public static IDisposable BindColorTo(this Material input, IReactiveProperty<Color> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Color>[] filters)
        { return GenericBindings.Bind(() => input.color, x => input.color = x, property, bindingType, filters); }

        public static IDisposable BindColorTo(this Material input, Func<Color> getter, Action<Color> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Color>[] filters)
        { return GenericBindings.Bind(() => input.color, x => input.color = x, getter, setter, bindingType, filters); }

        public static IDisposable BindTextureTo(this Material input, IReactiveProperty<Texture> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Texture>[] filters)
        { return GenericBindings.Bind(() => input.mainTexture, x => input.mainTexture = x, property, bindingType, filters); }

        public static IDisposable BindColorTo(this Material input, Func<Texture> getter, Action<Texture> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Texture>[] filters)
        { return GenericBindings.Bind(() => input.mainTexture, x => input.mainTexture = x, getter, setter, bindingType, filters); }

        public static IDisposable BindShaderTo(this Material input, IReactiveProperty<Shader> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Shader>[] filters)
        { return GenericBindings.Bind(() => input.shader, x => input.shader = x, property, bindingType, filters); }

        public static IDisposable BindShaderTo(this Material input, Func<Shader> getter, Action<Shader> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Shader>[] filters)
        { return GenericBindings.Bind(() => input.shader, x => input.shader = x, getter, setter, bindingType, filters); }
    }
}