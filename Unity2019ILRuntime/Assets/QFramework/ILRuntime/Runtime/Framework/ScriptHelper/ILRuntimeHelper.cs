using System;
using System.Collections.Generic;
using System.IO;
using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Generated;
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
            //AdapterRegister.RegisterCrossBindingAdaptor(AppDomain);
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