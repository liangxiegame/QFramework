using System;
using System.Collections.Generic;
using System.Threading;
using ILRuntime.Runtime.Intepreter;
using LitJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = System.Object;

namespace QFramework
{
    public class ILRuntimeDelegateHelper
    {
        //跨域委托调用，注册委托的适配器
        public static void RegisterDelegate(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            #region Method
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.Timers.ElapsedEventArgs>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean?>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Reflection.MethodInfo>();
            appdomain.DelegateManager
                .RegisterFunctionDelegate<ILTypeInstance, Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<List<Object>>();
            appdomain.DelegateManager
                .RegisterMethodDelegate<IDictionary<String, UnityEngine.Object>>();
            appdomain.DelegateManager.RegisterMethodDelegate<Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.UnhandledExceptionEventArgs>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<Boolean, GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<Int32, Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<String>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<UIBehaviour, Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<Transform, Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<GameObject, Action>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int64>();
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });
            
            #endregion

            #region Function
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.Int64, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int64, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Reflection.MethodInfo, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Type>();
            appdomain.DelegateManager.RegisterFunctionDelegate<string, string>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Reflection.ParameterInfo, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Threading.Tasks.Task<ILRuntime.Runtime.Intepreter.ILTypeInstance>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Object, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Reflection.ParameterInfo, System.Type>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Type, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Type>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Collections.Generic.IEnumerable<ILRuntime.Runtime.Intepreter.ILTypeInstance>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<float>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Threading.Tasks.Task>();
            appdomain.DelegateManager.RegisterFunctionDelegate<CoroutineAdapter.Adaptor, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<CoroutineAdapter.Adaptor, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<MonoBehaviourAdapter.Adaptor, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<QFramework.ILComponentBehaviour, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            #endregion

            #region Convertor

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ParameterizedThreadStart>((act) =>
            {
                return new System.Threading.ParameterizedThreadStart((obj) =>
                {
                    ((Action<System.Object>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.String>>((act) =>
            {
                return new System.Predicate<System.String>((obj) =>
                {
                    return ((Func<System.String, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ParameterizedThreadStart>((act) =>
            {
                return new System.Threading.ParameterizedThreadStart((obj) =>
                {
                    ((Action<System.Object>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
                {
                    ((Action<System.String>) act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
                {
                    ((Action<System.Boolean>) act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.WaitCallback>((act) =>
            {
                return new System.Threading.WaitCallback((state) => { ((Action<System.Object>) act)(state); });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction>(act =>
            {
                return new UnityAction(() => { ((Action) act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<Single>>(act =>
            {
                return new UnityAction<Single>(arg0 => { ((Action<Single>) act)(arg0); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.UnhandledExceptionEventHandler>((act) =>
            {
                return new System.UnhandledExceptionEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.UnhandledExceptionEventArgs>) act)(sender, e);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<Object>>(act =>
            {
                return new Predicate<Object>(obj => { return ((Func<Object, Boolean>) act)(obj); });
            });
            appdomain.DelegateManager
                .RegisterDelegateConvertor<Predicate<ILTypeInstance>>(act =>
                {
                    return new Predicate<ILTypeInstance>(obj =>
                    {
                        return ((Func<ILTypeInstance, Boolean>) act)(obj);
                    });
                });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<Int32>>(act =>
            {
                return new UnityAction<Int32>(arg0 => { ((Action<Int32>) act)(arg0); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Action<JsonData>>(action =>
            {
                return new Action<JsonData>(a => { ((Action<JsonData>) action)(a); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction>(act =>
            {
                return new UnityAction(() => {  ( (Action) act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<ThreadStart>(act =>
            {
                return new ThreadStart(() => { ((Action) act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<CoroutineAdapter.Adaptor>>(
                (act) =>
                {
                    return new System.Predicate<CoroutineAdapter.Adaptor>((obj) =>
                    {
                        return ((Func<CoroutineAdapter.Adaptor, System.Boolean>) act)(obj);
                    });
                });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<MonoBehaviourAdapter.Adaptor>>((act) =>
            {
                return new System.Predicate<MonoBehaviourAdapter.Adaptor>((obj) =>
                {
                    return ((Func<MonoBehaviourAdapter.Adaptor, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Timers.ElapsedEventHandler>((act) =>
            {
                return new System.Timers.ElapsedEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.Timers.ElapsedEventArgs>)act)(sender, e);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>>>((act) =>
            {
                return new System.Predicate<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>>((obj) =>
                {
                    return ((Func<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Boolean>)act)(obj);
                });
            });
            #endregion
        }
    }
}