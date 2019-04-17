/****************************************************************************
 * Copyright (c) 2018.12 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

namespace QFramework
{
	public class LuaComponent : MonoBehaviour
	{
		protected static class FuncName
		{
			public static readonly string Awake     = "Awake";
			public static readonly string OnEnable  = "OnEnable";
			public static readonly string Start     = "Start";
			public static readonly string Update    = "Update";
			public static readonly string OnDisable = "OnDisable";
			public static readonly string OnDestroy = "OnDestroy";
		};

		public static bool isFirstLaunch = true;

		/// <summary>  
		/// 提供给外部手动执行LUA脚本的接口  
		/// </summary>  
		public bool Initilize(string path)
		{
			LuaPath = path;
			Init();
			return true;
		}

		//lua路径，不用填缀名，可以是bundle
		[Tooltip("script path")] public string LuaPath;

		public string LuaFilePath;

		public LuaTable LuaModule
		{
			get { return mSelfLuaTable; }
		}


		public string LuaClass
		{
			get { return LuaClassName; }
		}


		private LuaTable mSelfLuaTable = null;
		private string   LuaClassName  = null;

		//初始化函数，可以被重写，已添加其他
		protected virtual bool Init()
		{		
			mSelfLuaTable = LuaMain.getInstance().addLuaFile(LuaPath, gameObject);
			LuaClassName = CallLuaFunctionRString("getClassName");
			
			mSelfLuaTable["gameObject"] = gameObject;
			mSelfLuaTable["transform"] = transform;


			// if (gameObject.GetComponent<Button>() != null)
			// {
			// 	gameObject.GetComponent<Button>().onClick.AddListener(
			// 		onClick
			// 	);
			// }

			return true;
		}

		private string CallLuaFunctionRString(string name, params object[] args)
		{
			string resault = null;
			if (mSelfLuaTable != null)
			{
				LuaFunction func = mSelfLuaTable.GetLuaFunction(name);
				if (null == func)
				{
					return resault;
				}

				func.BeginPCall();
				func.Push(mSelfLuaTable);
				foreach (var o in args)
				{
					func.Push(o);
				}

				func.PCall();
				resault = func.CheckString();
				func.EndPCall();
			}

			return resault;
		}


		public void CallLuaFunction(string name, params object[] args)
		{
			if (mSelfLuaTable != null)
			{
				LuaFunction func = mSelfLuaTable.GetLuaFunction(name);
				if (null == func)
				{
					return;
				}

				func.BeginPCall();
				func.Push(mSelfLuaTable);
				foreach (var o in args)
				{
					func.Push(o);
				}

				func.PCall();
				func.EndPCall();
			}
		}

		//void Awake()
		//{
		//	if (Initilize(LuaPath))
		//		CallLuaFunction(LuaMain.FuncName.Awake);
		//}

		//void OnEnable()
		//{
		//	CallLuaFunction(LuaMain.FuncName.OnEnable);
		//}

		//void Start()
		//{
		//	CallLuaFunction(LuaMain.FuncName.Start);
		//}

		//void Update()
		//{
		//	CallLuaFunction(LuaMain.FuncName.Update);
		//}

		//void OnDisable()
		//{
		//	CallLuaFunction(LuaMain.FuncName.OnDisable);
		//}

		//void OnDestroy()
		//{
		//	CallLuaFunction(LuaMain.FuncName.OnDestroy);

		//	if (null != mSelfLuaTable)
		//	{
		//		mSelfLuaTable.Dispose();
		//		mSelfLuaTable = null;
		//	}
		//}

        public void DisposeLuaTable(){
              if (null != mSelfLuaTable)
              {
                  mSelfLuaTable.Dispose();
                  mSelfLuaTable = null;
              }
        }

		// void onClick()
		// {
		// 	CallLuaFunction(LuaMain.FuncName.onClick);
		// }
	}
}