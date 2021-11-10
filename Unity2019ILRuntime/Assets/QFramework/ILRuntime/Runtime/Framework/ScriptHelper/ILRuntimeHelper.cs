using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;
using LitJson;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace QFramework
{
    public static class ILRuntimeHelper
    {
        public static AppDomain AppDomain { get; private set; }
        public static bool IsRunning { get; private set; }

        static private MemoryStream msDll = null;
        static private MemoryStream msPdb = null;

        public static void LoadHotfix(byte[] dllBytes, byte[] pdbBytes = null, bool isRegisterBindings = true)
        {
            //
            IsRunning = true;
   
            //
            AppDomain = new AppDomain();
            msDll = new MemoryStream(dllBytes);
            if (pdbBytes != null)
                msPdb = new MemoryStream(pdbBytes);
            AppDomain.LoadAssembly(msDll, msPdb, new PdbReaderProvider());
            
            //绑定的初始化
            //ada绑定
            //是否注册各种binding
            if (isRegisterBindings)
            {
                CLRBindings.Initialize(AppDomain);
                CLRManualBindings.Initialize(AppDomain);
                // ILRuntime.Runtime.Generated.PreCLRBuilding.Initialize(AppDomain);
            }

            ILRuntimeRedirectHelper.RegisterMethodRedirection(AppDomain);
            ILRuntimeDelegateHelper.RegisterDelegate(AppDomain);
            ILRuntimeValueTypeBinderHelper.Register(AppDomain);

            if (Application.isEditor)
            {
                AppDomain.DebugService.StartDebugService(56000);
            }
        }

        private static Dictionary<string,Type> hotfixType = null;
        
        public static IEnumerable<Type> GetHotfixTypes()
        {
            if (hotfixType == null)
            {
                hotfixType = new Dictionary<string, Type>();
                foreach (var v in AppDomain.LoadedTypes)
                {
                    hotfixType.Add(v.Key,v.Value.ReflectionType);
                }
            }
            
            return hotfixType.Values;
        }

        public static Type GetCLRType(object obj)
        {
            //如果是继承了主项目的热更的类型
            if (obj is CrossBindingAdaptorType adaptor)
            {
                return adaptor.ILInstance.Type.ReflectionType;
            }
            //如果是热更的类型
            if (obj is ILTypeInstance ilInstance)
            {
                return ilInstance.Type.ReflectionType;
            }

            return obj.GetType();
        }
        
        public static Type GetCLRType(Type type)
        {
            if (type is ILRuntimeType runtimeType)
                return runtimeType.ILType.ReflectionType;
            if (type is ILRuntimeWrapperType wrapperType)
                return wrapperType.RealType;
            return type;
        }
        
        public static Type GetCLRType(this IType type)
        {
            if (type is CLRType clrType)
            {
                return clrType.TypeForCLR;
            }
            return type.ReflectionType;
        }

        public static Type GetType(string name)
        {
            if (hotfixType == null)
                GetHotfixTypes();
            return hotfixType.TryGetValue(name, out var type) ? type : null;
        }

        public static void Close()
        {
            if (msDll != null)
            {
                msDll.Dispose();
            }

            if (msPdb != null)
            {
                msPdb.Dispose();
            }
        }
    }
}