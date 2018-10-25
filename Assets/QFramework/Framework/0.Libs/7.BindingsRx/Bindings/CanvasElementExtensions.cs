using System;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class CanvasElementExtensions
    {
        public static IDisposable BindPositionTo(this ICanvasElement input, IReactiveProperty<Vector3> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return input.transform.BindPositionTo(property, bindingType, filters).AddTo(input as MonoBehaviour); }

        public static IDisposable BindPositionTo(this ICanvasElement input, Func<Vector3> getter, Action<Vector3> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return input.transform.BindPositionTo(getter, setter, bindingType, filters).AddTo(input as MonoBehaviour); }

        public static IDisposable BindRotationTo(this ICanvasElement input, IReactiveProperty<Quaternion> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Quaternion>[] filters)
        { return input.transform.BindRotationTo(property, bindingType, filters).AddTo(input as MonoBehaviour); }

        public static IDisposable BindRotationTo(this ICanvasElement input, Func<Quaternion> getter, Action<Quaternion> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Quaternion>[] filters)
        { return input.transform.BindRotationTo(getter, setter, bindingType, filters).AddTo(input as MonoBehaviour); }

        public static IDisposable BindScaleTo(this ICanvasElement input, IReactiveProperty<Vector3> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return input.transform.BindScaleTo(property, bindingType, filters).AddTo(input as MonoBehaviour); }

        public static IDisposable BindScaleTo(this ICanvasElement input, Func<Vector3> getter, Action<Vector3> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return input.transform.BindScaleTo(getter, setter, bindingType, filters).AddTo(input as MonoBehaviour); }
    }
}