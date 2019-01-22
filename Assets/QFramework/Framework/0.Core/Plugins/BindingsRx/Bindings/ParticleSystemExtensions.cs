using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;

namespace BindingsRx.Bindings
{
    public static class ParticleSystemExtensions
    {
        public static IDisposable BindPlayTo(this ParticleSystem input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        {
            return GenericBindings.Bind(() => input.isPlaying, x => {
                if (x) { input.Play(); }
                else { input.Stop(); }
            }, property, bindingType, filters).AddTo(input);
        }

        public static IDisposable BindPlayTo(this ParticleSystem input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        {
            return GenericBindings.Bind(() => input.isPlaying, x =>
            {
                if (x) { input.Play(); }
                else { input.Stop(); }
            }, getter, setter, bindingType, filters).AddTo(input);
        }

        public static IDisposable BindTimeTo(this ParticleSystem input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.time, x => input.time = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindTimeTo(this ParticleSystem input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.time, x => input.time = x, getter, setter, bindingType, filters).AddTo(input); }
    }
}