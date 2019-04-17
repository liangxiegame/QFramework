using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using QFramework;
using UnityEngine.UI;
#if UNITY_EDITOR	
using UnityEditor;
#endif

namespace QFramework
{
	public class LuaMain
	{
		//lua环境，需要在使用前给其赋值
		private static LuaState mLuaState = null;

		public static string[] loadBundles      = { };

		public static bool nextClearBundles = false;

		//单例
		private static LuaMain mInstance;

		private string LuaEnterFile      = "Framework/init.lua";
		private string LuaEnterDebugFile = "Framework/init.lua";

		//函数名字定义
		public static class FuncName
		{
			public static readonly string createLuaFile = "CreateLuaFile";
			public static readonly string Awake         = "Awake";
			public static readonly string OnEnable      = "OnEnable";
			public static readonly string Start         = "Start";
			public static readonly string Update        = "Update";
			public static readonly string OnDisable     = "OnDisable";
			public static readonly string OnDestroy     = "OnDestroy";


			//ar
			public static readonly string OnTrackingFound = "OnTrackingFound";
			public static readonly string OnTrackingLost  = "OnTrackingLost";

			//Button
			public static readonly string onClick = "onClick";

		};

		//预存函数提高效率
		protected Dictionary<string, LuaFunction> mDictFunc = new Dictionary<string, LuaFunction>();

		public static LuaMain getInstance()
		{
			if (mInstance == null)
			{
				mInstance = new LuaMain();
				mInstance.init();
			}

			return mInstance;
		}

		private static GameObject luaCoroutine = null;

		//初始化
		private void init()
		{
#if UNITY_EDITOR
            //LuaFileUtils.Instance.beZip = UseAssetBundleLuaInEditor;
            LuaFileUtils.Instance.beZip = false;
#else
				LuaFileUtils.Instance.beZip = true;
			#endif
			mLuaState = new LuaState();
			mLuaState.LogGC = false;
			mLuaState.Start();
			LuaBinder.Bind(mLuaState);
			mLuaState.DoFile(LuaFileUtils.Instance.beZip ? LuaEnterFile : LuaEnterDebugFile);

			//协程
//			luaCoroutine = GameObject.Find("LuaCoroutine");
//			if (luaCoroutine == null)
//			{
//				luaCoroutine = new GameObject();
//				luaCoroutine.gameObject.name = "LuaCoroutine";
//				luaCoroutine.AddComponent<LuaCoroutineComponent>();
				UnityEngine.Object.DontDestroyOnLoad(luaCoroutine);
//			}

//			LuaCoroutine.Register(mLuaState, luaCoroutine.GetComponent<LuaCoroutineComponent>());

			addFunc(FuncName.createLuaFile);

		}


		//保存函数
		private bool addFunc(string funcName)
		{
			var func = mLuaState.GetFunction(funcName);
			if (null == func)
			{
				return false;
			}

			mDictFunc.Add(funcName, func);
			return true;
		}

		public LuaTable addLuaFile(params object[] args)
		{
			LuaFunction func = mLuaState.GetFunction(FuncName.createLuaFile);
			if (null == func)
			{
				return null;
			}

			func.BeginPCall();
			foreach (var o in args)
			{
				func.Push(o);
			}

			func.PCall();
			LuaTable table = func.CheckLuaTable();
			func.EndPCall();
			return table;
		}

		//销毁
		public void Destroy()
		{
			//记得释放资源
			foreach (var pair in mDictFunc)
			{
				pair.Value.Dispose();
			}

			mDictFunc.Clear();
			mLuaState.Dispose();
		}

		public static void AddClick(GameObject go, LuaFunction luafunc)
		{
			if (go == null || luafunc == null) return;
			go.GetComponent<Button>().onClick.AddListener(
				delegate() { luafunc.Call(go); }
			);
		}

		public static void RemoveClick(GameObject go)
		{
			if (go == null) return;
			go.GetComponent<Button>().onClick.RemoveAllListeners();
		}


		public static LuaComponent AddLuaComponent(GameObject go, string path)
		{
			LuaComponent luaComp = go.AddComponent<LuaComponent>();
			luaComp.Initilize(path); // 手动调用脚本运行，以取得LuaTable返回值  
			return luaComp;
		}


		public static LuaComponent GetLuaComponent(GameObject go, string LuaClassName)
		{
			LuaComponent[] comps = go.GetComponents<LuaComponent>();

			for (int i = 0; i < comps.Length; i++)
			{
				if (comps[i].LuaClass == LuaClassName)
					return comps[i];
			}

			return null;
//			return 
		}

		public static void AddEditorSearchPath(string path)
		{
#if UNITY_EDITOR
			if (mLuaState != null)
			{
				mLuaState.AddSearchPath(Application.dataPath + "/" + path);
			}
#endif
		}

		public static void Dispose()
		{
			mInstance = null;
			if (luaCoroutine != null)
			{
				GameObject.Destroy(luaCoroutine);
			}

			if (mLuaState != null)
			{
				mLuaState.Dispose();
				mLuaState = null;
			}

//			UnityEngine.SceneManagement.SceneManager.LoadScene ("ARBookFramework_BookMenu");
//			LuaMain.assetBundleLoaded = false;
//			LuaMain.getDisposeInstance().Destroy ();

		}

#if UNITY_EDITOR
		static int m_SimulateAssetBundleInEditor = -1;

		const string kSimulateAssetBundles = "UseLuaAssetBundles";

		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool UseAssetBundleLuaInEditor
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}
#else
		public static bool UseAssetBundleLuaInEditor 
		{
			get
			{
				return true;
			}
			set
			{
				
			}
		}
#endif
		//----------------------外部接口----------------------
	}
}