using System;
using BindingsRx.Extensions;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;

namespace BindingsRx.Bindings
{
    public static class AnimatorExtensions
    {
        public static IDisposable BindRootMotionTo(this Animator input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.applyRootMotion, x => input.applyRootMotion = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindRootMotionTo(this Animator input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.applyRootMotion, x => input.applyRootMotion = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindRootPositionTo(this Animator input, IReactiveProperty<Vector3> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return GenericBindings.Bind(() => input.rootPosition, x => input.rootPosition = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindRootPositionTo(this Animator input, Func<Vector3> getter, Action<Vector3> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return GenericBindings.Bind(() => input.rootPosition, x => input.rootPosition = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindRootPositionTo(this Animator input, IReactiveProperty<Quaternion> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Quaternion>[] filters)
        { return GenericBindings.Bind(() => input.rootRotation, x => input.rootRotation = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindRootPositionTo(this Animator input, Func<Quaternion> getter, Action<Quaternion> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Quaternion>[] filters)
        { return GenericBindings.Bind(() => input.rootRotation, x => input.rootRotation = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindFireEventsTo(this Animator input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.fireEvents, x => input.fireEvents = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindFireEventsTo(this Animator input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.fireEvents, x => input.fireEvents = x, getter, setter, bindingType, filters).AddTo(input); }
        
        public static IDisposable BindStabilizeFeetTo(this Animator input, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.stabilizeFeet, x => input.stabilizeFeet = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindStabilizeFeetTo(this Animator input, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.stabilizeFeet, x => input.stabilizeFeet = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindBoolTo(this Animator input, string parameter, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.GetBool(parameter), x => input.SetBool(parameter, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindBoolTo(this Animator input, string parameter, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.GetBool(parameter), x => input.SetBool(parameter, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindBoolTo(this Animator input, int parameter, IReactiveProperty<bool> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.GetBool(parameter), x => input.SetBool(parameter, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindBoolTo(this Animator input, int parameter, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(() => input.GetBool(parameter), x => input.SetBool(parameter, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindFloatTo(this Animator input, string parameter, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetFloat(parameter), x => input.SetFloat(parameter, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindFloatTo(this Animator input, string parameter, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetFloat(parameter), x => input.SetFloat(parameter, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindFloatTo(this Animator input, int parameter, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetFloat(parameter), x => input.SetFloat(parameter, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindFloatTo(this Animator input, int parameter, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetFloat(parameter), x => input.SetFloat(parameter, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIntTo(this Animator input, string parameter, IReactiveProperty<int> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.GetInteger(parameter), x => input.SetInteger(parameter, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIntTo(this Animator input, string parameter, Func<int> getter, Action<int> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.GetInteger(parameter), x => input.SetInteger(parameter, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIntTo(this Animator input, int parameter, IReactiveProperty<int> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.GetInteger(parameter), x => input.SetInteger(parameter, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIntTo(this Animator input, int parameter, Func<int> getter, Action<int> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.GetInteger(parameter), x => input.SetInteger(parameter, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindLayerWeightTo(this Animator input, int layer, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetLayerWeight(layer), x => input.SetLayerWeight(layer, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindLayerWeightTo(this Animator input, int layer, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetLayerWeight(layer), x => input.SetLayerWeight(layer, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindTriggerTo(this Animator input, string parameter, IReactiveProperty<bool> property, IFilter<bool>[] filters)
        { return GenericBindings.Bind(null, x => input.SetTrigger(parameter), property, BindingTypes.OneWay, filters).AddTo(input); }

        public static IDisposable BindTriggerTo(this Animator input, string parameter, Func<bool> getter, Action<bool> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<bool>[] filters)
        { return GenericBindings.Bind(null, x => input.SetTrigger(parameter), getter, setter, BindingTypes.OneWay, filters).AddTo(input); }

        public static IDisposable BindTriggerTo(this Animator input, int parameter, IReactiveProperty<bool> property, params IFilter<bool>[] filters)
        {
            return property
                .ApplyInputFilters(filters)
                .DistinctUntilChanged(x => x)
                .Subscribe(x =>
                {
                    input.SetTrigger(parameter);
                    property.Value = false;
                }).AddTo(input);
        }

        public static IDisposable BindTriggerTo(this Animator input, int parameter, Func<bool> getter, Action<bool> setter, params IFilter<bool>[] filters)
        {
            return Observable.EveryUpdate()
                .Select(x => getter())
                .ApplyInputFilters(filters)
                .DistinctUntilChanged(x => x)
                .Subscribe(x =>
                {
                    input.SetTrigger(parameter);
                    setter(false);
                }).AddTo(input);
        }

        public static IDisposable BindSpeedTo(this Animator input, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.speed, x => input.speed = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindSpeedTo(this Animator input, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.speed, x => input.speed = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkHintPositionTo(this Animator input, AvatarIKHint ikHint, IReactiveProperty<Vector3> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return GenericBindings.Bind(() => input.GetIKHintPosition(ikHint), x => input.SetIKHintPosition(ikHint, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkHintPositionTo(this Animator input, AvatarIKHint ikHint, Func<Vector3> getter, Action<Vector3> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return GenericBindings.Bind(() => input.GetIKHintPosition(ikHint), x => input.SetIKHintPosition(ikHint, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkPositionTo(this Animator input, AvatarIKGoal ikGoal, IReactiveProperty<Vector3> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return GenericBindings.Bind(() => input.GetIKPosition(ikGoal), x => input.SetIKPosition(ikGoal, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkPositionTo(this Animator input, AvatarIKGoal ikGoal, Func<Vector3> getter, Action<Vector3> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Vector3>[] filters)
        { return GenericBindings.Bind(() => input.GetIKPosition(ikGoal), x => input.SetIKPosition(ikGoal, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkRotationTo(this Animator input, AvatarIKGoal ikGoal, IReactiveProperty<Quaternion> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<Quaternion>[] filters)
        { return GenericBindings.Bind(() => input.GetIKRotation(ikGoal), x => input.SetIKRotation(ikGoal, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkRotationTo(this Animator input, AvatarIKGoal ikGoal, Func<Quaternion> getter, Action<Quaternion> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<Quaternion>[] filters)
        { return GenericBindings.Bind(() => input.GetIKRotation(ikGoal), x => input.SetIKRotation(ikGoal, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkPositionWeightnTo(this Animator input, AvatarIKGoal ikGoal, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetIKPositionWeight(ikGoal), x => input.SetIKPositionWeight(ikGoal, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkPositionWeightnTo(this Animator input, AvatarIKGoal ikGoal, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetIKPositionWeight(ikGoal), x => input.SetIKPositionWeight(ikGoal, x), getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkRotationWeightTo(this Animator input, AvatarIKGoal ikGoal, IReactiveProperty<float> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetIKRotationWeight(ikGoal), x => input.SetIKRotationWeight(ikGoal, x), property, bindingType, filters).AddTo(input); }

        public static IDisposable BindIkRotationWeightTo(this Animator input, AvatarIKGoal ikGoal, Func<float> getter, Action<float> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<float>[] filters)
        { return GenericBindings.Bind(() => input.GetIKRotationWeight(ikGoal), x => input.SetIKRotationWeight(ikGoal, x), getter, setter, bindingType, filters).AddTo(input); }
    }
}