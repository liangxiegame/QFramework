using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace QFramework
{

    public class ILRuntimeScript : IScript
    {
        private Assembly Assembly { get; set; }

        // 加載 dll
        // isregisterBindings 為 false 時，是生成 binding的時候
        private void LoadDll(string dllPath, bool isRegisterBindings = true)
        {
            switch (ILRuntimeScriptSetting.Default.HotfixRunMode)
            {
                case HotfixCodeRunMode.ILRuntime:
                    ILRuntimeHelper.LoadHotfix(dllPath, isRegisterBindings);
                    break;
                case HotfixCodeRunMode.Reflection:
                {
                    var asmdefName = ILRuntimeScriptSetting.Default.HotfixAsmdefName;
                    try
                    {
                        if (string.IsNullOrEmpty(asmdefName))
                        {
                            Assembly = AppDomain.CurrentDomain.GetAssemblies()
                                .First(assembly => assembly.GetName().Name.EndsWith("@hotfix"));
                        }
                        else
                        {
                            Assembly = AppDomain.CurrentDomain.GetAssemblies()
                                .First(assembly => assembly.GetName().Name == asmdefName);
                        }
                    }
                    finally
                    {
                        if (Assembly == null)
                        {
                            Log.E($"程序集加载失败--{(string.IsNullOrEmpty(asmdefName) ? "检查有没有@hotfix的程序集" : $@"检查设置中的程序集名字{asmdefName}")}");
                        }
                    }
                    break;
                }
            }
        }


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
            string dllPath = "";
            //反射
            if (ILRuntimeScriptSetting.Default.HotfixRunMode == HotfixCodeRunMode.ILRuntime)
            {
                //ILRuntime基于文件流，所以不支持file操作的，得拷贝到支持File操作的目录

                //这里情况比较复杂,Mobile上基本认为Persistent才支持File操作,
                //可寻址目录也只有 StreamingAsset
                var firstPath = ILRuntimeScriptSetting.DllFilePersistentFullPath;
                var secondPath = ILRuntimeScriptSetting.DllFileStreamingFullPath;

                // if (!File.Exists(firstPath)) //仅当指定的路径不存在(或者不支持File操作)时,再进行可寻址
                // {
                //     //验证 可寻址目录2
                //     var source = secondPath;
                //     var copyto = firstPath;
                //     Load(source, bytes =>
                //     {
                //         copyto.CreateDirIfNotExists4FilePath();
                //         File.WriteAllBytes(copyto, bytes);
                //
                //         //解释执行模式
                //         LoadDll(copyto);
                //         loadDone.InvokeGracefully();
                //
                //     }, e => { Debug.LogError("可寻址目录不包括DLL:" + source); });
                //     return;
                // }
                dllPath = secondPath;
            }
            LoadDll(dllPath);
            loadDone?.Invoke();
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