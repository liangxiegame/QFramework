using System;
using System.IO;
using System.Reflection;
using BDFramework;
using UnityEngine;

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
                    var bytes = File.ReadAllBytes(dllPath);
                    var bytes2 = File.ReadAllBytes(dllPath + ".mdb");
                    Assembly = Assembly.Load(bytes, bytes2);
                    break;
                }
                case HotfixCodeRunMode.Editor:
                    // 等分了asmdef 则去获取asmdef生成的程序集
                    Assembly = Assembly.GetExecutingAssembly();
                    break;
            }
        }

        // 加載 dll
        // isregisterBindings 為 false 時，是生成 binding的時候
        public void LoadDll(byte[] bytes)
        {
            Assembly = Assembly.Load(bytes);
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
            //初始化资源加载
            string dllPath = "";

            if (dllPath != "")
            {

                //反射
                if (ILRuntimeScriptSetting.Default.HotfixRunMode == HotfixCodeRunMode.Reflection)
                {
                    //反射模式只支持Editor PC Android
                    if (File.Exists(dllPath)) //支持File操作 或者存在
                    {
                        LoadDll(dllPath);
                        loadDone?.Invoke();
                    }
                    else
                    {
                        //不支持file操作 或者不存在,继续尝试
                        var path = dllPath;

                        if (Application.isEditor)
                        {
                            path = "file://" + path;
                        }
                        Load(path,bytes =>
                        {
                            LoadDll(bytes);
                            loadDone.InvokeGracefully();

                        }, e => { Debug.LogError("DLL加载失败:" + e); });
                    }
                }
                //ILR
                else
                {
                    //ILRuntime基于文件流，所以不支持file操作的，得拷贝到支持File操作的目录

                    //这里情况比较复杂,Mobile上基本认为Persistent才支持File操作,
                    //可寻址目录也只有 StreamingAsset
                    var firstPath = ILRuntimeScriptSetting.DllFilePersistentFullPath;
                    var secondPath = ILRuntimeScriptSetting.DllFileStreamingFullPath;

                    if (!File.Exists(dllPath)) //仅当指定的路径不存在(或者不支持File操作)时,再进行可寻址
                    {
                        dllPath = firstPath;
                        if (!File.Exists(firstPath))
                        {
                            //验证 可寻址目录2
                            var source = secondPath;
                            var copyto = firstPath;
                            Debug.Log("复制到第一路径:" + source);
                            Load(source, bytes =>
                                {
                                    copyto.CreateDirIfNotExists4FilePath();
                                    File.WriteAllBytes(copyto, bytes);

                                    //解释执行模式
                                    ILRuntimeScriptSetting.Default.HotfixRunMode = HotfixCodeRunMode.ILRuntime;
                                    LoadDll(copyto);
                                    loadDone.InvokeGracefully();

                                }, e => { Debug.LogError("可寻址目录不包括DLL:" + source); });
                            return;
                        }
                    }

                    //解释执行模式
                    LoadDll(dllPath);
                    loadDone?.Invoke();
                }
            }
            else
            {
                // pc 模式
                ILRuntimeScriptSetting.Default.HotfixRunMode = HotfixCodeRunMode.Editor;
                LoadDll("");
                loadDone?.Invoke();
            }
        }

        private void Load(string path, Action<byte[]> cb, Action<Exception> e)
        {
            
        }

        public void Dispose()
        {
            ILRuntimeHelper.Close();
        }
    }
}