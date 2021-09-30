using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using LitJson;
using ProtoBuf.Meta;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using Object = UnityEngine.Object;

namespace QFramework
{
     /// <summary>
    /// 有参无返回值实例泛型方法 指针从1-n表示第n个参数，最后一个表示this指针 , 返回值使用this指针
    /// 无参则第一个为this指针
    ///
    /// 泛型Type  type is CLRType 是主项目 type.TypeForCLR
    ///             else type.ReflectionType 
    ///
    /// object is ILTypeInstance 是热更实例
    ///
    /// 有返回值时，push的指针如果是ins则使用ins，否则使用esp
    ///
    /// 重定向时父类和子类都要把方法注册进去
    /// </summary>
    public static class ILRuntimeRedirectHelper
    {
        public static unsafe void RegisterMethodRedirection(AppDomain appdomain)
        {
            Type[] args;
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
                                BindingFlags.DeclaredOnly;

            Type log = typeof(Log);
            foreach (var methodInfo in log.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "E":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, (intp, esp, stack, method, obj) =>
                            LogMsg(intp, esp, stack, method, obj, content => Log.E(content)));
                        break;
                    case "I":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, (intp, esp, stack, method, obj) =>
                            LogMsg(intp, esp, stack, method, obj, content => Log.I(content)));
                        break;
                    case "W":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, (intp, esp, stack, method, obj) =>
                            LogMsg(intp, esp, stack, method, obj, content => Log.W(content)));
                        break;
                }
            }
            
            //注册Add Component
            Type gameObjectType = typeof(GameObject);
            foreach (var methodInfo in gameObjectType.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "AddComponent" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, AddComponent);
                        break;
                    case "GetComponent" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent);
                        break;
                    case "GetComponent" when !methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent_1);
                        break;
                }
            }

            Type componentType = typeof(Component);
            foreach (var methodInfo in componentType.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "GetComponent" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent);
                        break;
                    case "GetComponent" when !methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent_1);
                        break;
                }
            }

            args = new[] {typeof(GameObject)};
            var getOrAdd1 = typeof(GameObjectExtension).GetMethod("GetOrAddComponent", flag, null, args, null);
            appdomain.RegisterCLRMethodRedirection(getOrAdd1, GetOrAddComponent);

            JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
        }
        
        private static unsafe StackObject* LogMsg(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj, Action<object> action)
        {
            AppDomain domain = intp.AppDomain;
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            var ptr_msg = ILIntepreter.Minus(esp, 2);
            var content = StackObject.ToObject(ptr_msg, domain, mStack);
            var message = content == null ? "null" : content.ToString();
            intp.Free(ptr_msg);
            string stackTrace = domain.DebugService.GetStackTrace(intp);
            message += "\n" + stackTrace;
            action(message);
            return ret;
        }

        #region Component

        /// <summary>
        /// Get的字符串参数重定向
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static unsafe StackObject* GetOrAddComponent(ILIntepreter __intp, StackObject* __esp,
            IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            var ins = StackObject.ToObject(ptr, __domain, __mStack);
            if (ins == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                try
                {
                    var res = getComponent(ins, genericArgument[0]);
                    //没有找到组件
                    if (res == null || res.Equals(null))
                    {
                        //添加组件
                        res = addComponent(ins as GameObject, genericArgument[0], __domain);
                    }
                    return ILIntepreter.PushObject(ptr, __mStack, res);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.E($"{e}\n{stackTrace}");
                }
            }

            return __esp;
        }

        /// <summary>
        /// AddComponent 实现
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                try
                {
                    object res = addComponent(instance, genericArgument[0], __domain);
                    return ILIntepreter.PushObject(ptr, __mStack, res);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.E($"{e}\n{stackTrace}");
                }
            }

            return __esp;
        }

        private static object addComponent(GameObject instance, IType type, AppDomain __domain)
        {

            object res;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                ILTypeInstance ilInstance = new ILTypeInstance(type as ILType, false);
                Type t = type.ReflectionType; //获取实际属性
                bool isMonoAdapter = t.BaseType?.FullName == typeof(MonoBehaviourAdapter.Adaptor).FullName;

                if (!isMonoAdapter && Type.GetType(t.BaseType.FullName) != null)
                {
                    Type adapterType = Type.GetType(t.BaseType?.FullName);
                    if (adapterType == null)
                    {
                        Log.E($"{t.FullName} need to generate adapter");
                        return null;
                    }

                    //直接反射赋值一波了
                    var clrInstance = instance.AddComponent(adapterType);
                    var ILInstance = t.GetField("instance",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    var AppDomain = t.GetField("appdomain",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    ILInstance.SetValue(clrInstance, ilInstance);
                    AppDomain.SetValue(clrInstance, __domain);
                    ilInstance.CLRInstance = clrInstance;
                    bool activated = false;
                    //不管是啥类型，直接invoke这个awake方法
                    var awakeMethod = clrInstance.GetType().GetMethod("Awake",
                        BindingFlags.Default | BindingFlags.Public
                                             | BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                             BindingFlags.NonPublic | BindingFlags.Static);
                    if (awakeMethod == null)
                    {
                        awakeMethod = t.GetMethod("Awake",
                            BindingFlags.Default | BindingFlags.Public
                                                 | BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                                 BindingFlags.NonPublic | BindingFlags.Static);
                    }
                    else
                    {
                        awakeMethod.Invoke(clrInstance, null);
                        activated = true;
                    }

                    if (awakeMethod == null)
                    {
                        Log.E($"{t.FullName}不包含Awake方法，无法激活，已跳过");
                    }
                    else if (!activated)
                    {
                        awakeMethod.Invoke(t, null);
                    }
                }
                else
                {
                    //接下来创建Adapter实例
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    clrInstance.ILInstance = ilInstance;
                    clrInstance.AppDomain = __domain;
                    //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;
                    clrInstance.Awake(); //因为Unity调用这个方法时还没准备好所以这里补调一次
                }

                res = ilInstance;

                var m = type.GetConstructor(ILRuntime.CLR.Utils.Extensions.EmptyParamList);
                if (m != null)
                {
                    __domain.Invoke(m, res, null);
                }
            }
            return res;
        }


        /// <summary>
        /// Get的字符串参数重定向
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static unsafe StackObject* GetComponent_1(ILIntepreter __intp, StackObject* __esp,
            IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            String type = (String) StackObject.ToObject(ptr_of_this_method, __domain, __mStack);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GameObject instance_of_this_method =
                (GameObject) (StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            object result_of_this_method = null;
            try
            {
                result_of_this_method = instance_of_this_method.GetComponent(type); //先从本地匹配

                if (result_of_this_method == null) //本地没再从热更匹配
                {
                    var typeName = __domain.LoadedTypes.Keys.ToList().Find(k => k.EndsWith(type));
                    if (typeName != null) //如果有这个热更类型
                    {
                        //适配器全查找出来，匹配ILTypeInstance的真实类型的FullName
                        var clrInstances = instance_of_this_method.GetComponents<CrossBindingAdaptorType>();
                        for (int i = 0; i < clrInstances.Length; i++)
                        {
                            var clrInstance = clrInstances[i];
                            if (clrInstance.ILInstance != null) //ILInstance为null, 表示是无效的MonoBehaviour，要略过
                            {
                                if (clrInstance.ILInstance.Type.ReflectionType.FullName == typeName)
                                {
                                    result_of_this_method = clrInstance.ILInstance; //交给ILRuntime的实例应该为ILInstance
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                Log.E($"{e}\n{stackTrace}");
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        /// <summary>
        /// GetComponent 的实现
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr = ILIntepreter.Minus(__esp, 1);
            //成员方法的第一个参数为this
            var ins = StackObject.ToObject(ptr, __domain, __mStack);
            if (ins == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                try
                {
                    var res = getComponent(ins, genericArgument[0]);
                    if (res != null)
                        return ILIntepreter.PushObject(ptr, __mStack, res);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.E($"{e}\n{stackTrace}");
                }
            }

            return __esp;
        }

        private static object getComponent(object ins, IType type)
        {
            GameObject instance;

            if (ins is GameObject)
            {
                instance = ins as GameObject;
            }
            else if (ins is Component)
            {
                instance = ((Component) ins).gameObject;
            }
            else if (ins is ILTypeInstance)
            {
                instance = FindGOFromHotClass(((ILTypeInstance) ins));
            }
            else
            {
                Debug.LogError($"[GetComponent错误] 不支持的参数类型：{ins.GetType().FullName}，" +
                               "请传参GameObject或继承MonoBehaviour的对象");
                return null;
            }

            object res = null;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                var clrInstances = instance.GetComponents<CrossBindingAdaptorType>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null) //ILInstance为null, 表示是无效的MonoBehaviour，要略过
                    {
                        if (clrInstance.ILInstance.Type == type ||
                            clrInstance.ILInstance.Type.ReflectionType.IsSubclassOf(type.ReflectionType))
                        {
                            res = clrInstance.ILInstance; //交给ILRuntime的实例应该为ILInstance
                            break;
                        }
                    }
                }
            }
            return res;
        }

        private static GameObject FindGOFromHotClass(ILTypeInstance instance)
        {
            var returnType = instance.Type;
            if (returnType.ReflectionType == typeof(MonoBehaviour))
            {
                var pi = returnType.ReflectionType.GetProperty("gameObject");
                return pi.GetValue(instance.CLRInstance) as GameObject;
            }

            if (returnType.ReflectionType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                var pi = returnType.ReflectionType.BaseType.GetProperty("gameObject");
                return pi.GetValue(instance.CLRInstance) as GameObject;
            }
            return null;
        }

        #endregion
    }
}