/****************************************************************************
 * Copyright (c) 2017 ~ 2019.11 liangxie
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

using System;
using System.Reflection;
#if UNITY_5_6_OR_NEWER
using UnityEngine;
using Object = UnityEngine.Object;
#endif

namespace QFramework
{


	public interface ISingleton
	{
		void OnSingletonInit();
	}
	
	public abstract class Singleton<T> : ISingleton where T : Singleton<T>
	{
		protected static T mInstance;

		static object mLock = new object();

		public static T Instance
		{
			get
			{
				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = SingletonCreator.CreateSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public virtual void Dispose()
		{
			mInstance = null;
		}

		public virtual void OnSingletonInit()
		{
		}
	}
	
	public static class SingletonCreator
	{
		public static T CreateSingleton<T>() where T : class, ISingleton
		{
			// 获取私有构造函数
			var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            
			// 获取无参构造函数
			var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

			if (ctor == null)
			{
				throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
			}

			// 通过构造函数，常见实例
			var retInstance = ctor.Invoke(null) as T;
			retInstance.OnSingletonInit();

			return retInstance;
		}
	}
	
	public static class SingletonProperty<T> where T : class, ISingleton
	{
		private static          T      mInstance;
		private static readonly object mLock = new object();

		public static T Instance
		{
			get
			{
				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = SingletonCreator.CreateSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public static void Dispose()
		{
			mInstance = null;
		}
	}

#if UNITY_5_6_OR_NEWER
	[Obsolete]
	public class QMonoSingletonPath : MonoSingletonPath
	{
		public QMonoSingletonPath(string pathInHierarchy) : base(pathInHierarchy)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class MonoSingletonPath : Attribute
	{
		private string mPathInHierarchy;

		public MonoSingletonPath(string pathInHierarchy)
		{
			mPathInHierarchy = pathInHierarchy;
		}

		public string PathInHierarchy
		{
			get { return mPathInHierarchy; }
		}
	}

	public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
	{
		protected static T mInstance;

		public static T Instance
		{
			get
			{
				if (mInstance == null && !mOnApplicationQuit)
				{
					mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
				}

				return mInstance;
			}
		}

		public virtual void OnSingletonInit()
		{
		}

		public virtual void Dispose()
		{
			if (MonoSingletonCreator.IsUnitTestMode)
			{
				var curTrans = transform;
				do
				{
					var parent = curTrans.parent;
					DestroyImmediate(curTrans.gameObject);
					curTrans = parent;
				} while (curTrans != null);

				mInstance = null;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		protected static bool mOnApplicationQuit = false;

		protected virtual void OnApplicationQuit()
		{
			mOnApplicationQuit = true;
			if (mInstance == null) return;
			Destroy(mInstance.gameObject);
			mInstance = null;
		}
		protected virtual void OnDestroy()
		{
			mInstance = null;
		}
	}

	public static class MonoSingletonCreator
	{
		public static bool IsUnitTestMode { get; set; }

		public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
		{
			T instance = null;

			if (!IsUnitTestMode && !Application.isPlaying) return instance;
			instance = Object.FindObjectOfType<T>();

			if (instance != null)
			{
				instance.OnSingletonInit();
				return instance;
			}

			MemberInfo info = typeof(T);
			var attributes = info.GetCustomAttributes(true);
			foreach (var atribute in attributes)
			{
				var defineAttri = atribute as MonoSingletonPath;
				if (defineAttri == null)
				{
					continue;
				}

				instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, true);
				break;
			}

			if (instance == null)
			{
				var obj = new GameObject(typeof(T).Name);
				if (!IsUnitTestMode)
					Object.DontDestroyOnLoad(obj);
				instance = obj.AddComponent<T>();
			}

			instance.OnSingletonInit();
			return instance;
		}

		private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : MonoBehaviour
		{
			var obj = FindGameObject(path, true, dontDestroy);
			if (obj == null)
			{
				obj = new GameObject("Singleton of " + typeof(T).Name);
				if (dontDestroy && !IsUnitTestMode)
				{
					Object.DontDestroyOnLoad(obj);
				}
			}

			return obj.AddComponent<T>();
		}

		private static GameObject FindGameObject(string path, bool build, bool dontDestroy)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			var subPath = path.Split('/');
			if (subPath == null || subPath.Length == 0)
			{
				return null;
			}

			return FindGameObject(null, subPath, 0, build, dontDestroy);
		}

		private static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build,
			bool dontDestroy)
		{
			GameObject client = null;

			if (root == null)
			{
				client = GameObject.Find(subPath[index]);
			}
			else
			{
				var child = root.transform.Find(subPath[index]);
				if (child != null)
				{
					client = child.gameObject;
				}
			}

			if (client == null)
			{
				if (build)
				{
					client = new GameObject(subPath[index]);
					if (root != null)
					{
						client.transform.SetParent(root.transform);
					}

					if (dontDestroy && index == 0 && !IsUnitTestMode)
					{
						GameObject.DontDestroyOnLoad(client);
					}
				}
			}

			if (client == null)
			{
				return null;
			}

			return ++index == subPath.Length ? client : FindGameObject(client, subPath, index, build, dontDestroy);
		}
	}
	
	public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
	{
		private static T mInstance;

		public static T Instance
		{
			get
			{
				if (null == mInstance)
				{
					mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
				}

				return mInstance;
			}
		}

		public static void Dispose()
		{
			if (MonoSingletonCreator.IsUnitTestMode)
			{
				Object.DestroyImmediate(mInstance.gameObject);
			}
			else
			{
				Object.Destroy(mInstance.gameObject);
			}

			mInstance = null;
		}
	}
#endif
}