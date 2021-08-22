using System.IO;
using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Generated;
using LitJson;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace BDFramework
{
    static public class ILRuntimeHelper
    {
        public static AppDomain AppDomain { get; private set; }
        public static bool IsRunning { get; private set; }

        static private FileStream fsDll = null;
        static private FileStream fsPdb = null;

        public static void LoadHotfix(string dllPath, bool isRegisterBindings = true)
        {
            //
            IsRunning = true;
            string pdbPath = dllPath.Replace(".dll", ".pdb");

            Debug.Log("DLL加载路径:" + dllPath);
            //
            AppDomain = new AppDomain();
            if (File.Exists(pdbPath))
            {
                //这里的流不能释放，头铁的老哥别试了
                fsDll = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
                fsPdb = new FileStream(pdbPath, FileMode.Open, FileAccess.Read);
                AppDomain.LoadAssembly(fsDll, fsPdb, new PdbReaderProvider());
            }
            else
            {
                //这里的流不能释放，头铁的老哥别试了
                fsDll = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
                AppDomain.LoadAssembly(fsDll);
            }


            //绑定的初始化
            //ada绑定
            AdapterRegister.RegisterCrossBindingAdaptor(AppDomain);
            //delegate绑定
            ILRuntimeDelegateHelper.Register(AppDomain);
            //值类型绑定
            AppDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            AppDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            AppDomain.RegisterValueTypeBinder(typeof(Vector4), new Vector4Binder());
            AppDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
           
            
            
            
            //是否注册各种binding
            if (isRegisterBindings)
            {
                CLRBindings.Initialize(AppDomain);
                CLRManualBindings.Initialize(AppDomain);
                // ILRuntime.Runtime.Generated.PreCLRBuilding.Initialize(AppDomain);
            }

            JsonMapper.RegisterILRuntimeCLRRedirection(AppDomain);

            if (Application.isEditor)
            {
                AppDomain.DebugService.StartDebugService(56000);
                Debug.Log("热更调试器 准备待命~");
            }
        }


        public static void Close()
        {
            if (fsDll != null)
            {
                fsDll.Dispose();
            }

            if (fsPdb != null)
            {
                fsPdb.Dispose();
            }
        }
    }
}