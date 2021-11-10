using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public class ILRuntimeScript : IScript
    {
        private Assembly Assembly { get; set; }


        public void CallStaticMethod(string typeOrFileName, string methodName, params object[] args)
        {
            if (ILRuntimeScriptSetting.Default.HotfixRunMode == HotfixCodeRunMode.ILRuntime)
            {
                ILRuntimeHelper.AppDomain.Invoke(typeOrFileName, methodName, null, args);
            }
            else
            {
                var type = Assembly.GetType(typeOrFileName);
                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, args);
            }
        }

        public void LoadScript(Action loadDone)
        {
            string dllPath = ILRuntimeScriptSetting.Default.HotfixDllPath;
            string pdbPath = ILRuntimeScriptSetting.Default.HotfixPdbPath;
            var resloader = ResLoader.Allocate();
            switch (ILRuntimeScriptSetting.Default.HotfixRunMode)
            {
                case HotfixCodeRunMode.ILRuntime:
                    ILRuntimeHelper.LoadHotfix(resloader.LoadSync<TextAsset>(dllPath).bytes, resloader.LoadSync<TextAsset>(pdbPath).bytes);
                    break;
                case HotfixCodeRunMode.Reflection:
                {
                    var asmdefName = ILRuntimeScriptSetting.Default.HotfixDllName;
                    try
                    {
                        if (Application.isEditor)
                        {
                            Assembly = AssemblyManager.GetAssembly(asmdefName);
                        }else if (Application.platform == RuntimePlatform.Android)
                        {
                            Assembly = Assembly.Load(resloader.LoadSync<TextAsset>(dllPath).bytes);
                        }else if (Application.platform == RuntimePlatform.OSXPlayer)
                        {
                            //TODO 苹果待测试
                        }
                    }
                    finally
                    {
                        if (Assembly == null)
                        {
                            Log.E($"程序集加载失败--检查设置中的程序集名字{asmdefName}");
                        }
                    }
                    break;
                }
            }

            //ReflectionHelper.InitCustomFunc(InstanceFunc, ILRuntimeHelper.GetType, ILRuntimeHelper.GetCLRType, ILRuntimeHelper.GetCLRType);
            loadDone?.Invoke();
        }
        
        private static object InstanceFunc(Type type, object[] args)
        {
            if (type.Assembly.GetName().Name == ILRuntimeScriptSetting.Default.HotfixDllName)
            {
                ILTypeInstance ins = ILRuntimeHelper.AppDomain.Instantiate(type.FullName, args);
                return ins.CLRInstance;
            }

            return Activator.CreateInstance(type, args);
        }

        private async void Load(string path, Action<byte[]> cb, Action<Exception> e)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            www.SendWebRequest();
            while (!www.isDone)
            {
                await Task.Yield();
            }
            if (www.isHttpError || www.isNetworkError)
            {
                e?.Invoke(new WebException("www.error"));
                return;
            }
            cb(www.downloadHandler.data);
        }

        public void Dispose()
        {
            ILRuntimeHelper.Close();
        }
    }
}