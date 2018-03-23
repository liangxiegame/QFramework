/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
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

using System.Reflection;
using UnityEngine;

namespace QFramework
{
	public sealed class QSingletonCreator
	{
		/// <summary>
		/// for unit test
		/// </summary>
		private static bool mIsUnitTestMode;

		public static bool IsUnitTestMode
		{
			get { return mIsUnitTestMode; }
			set { mIsUnitTestMode = value; }
		}

		public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
		{
			T instance = null;

			if (instance != null || (!mIsUnitTestMode && !Application.isPlaying)) return instance;
			instance = GameObject.FindObjectOfType(typeof(T)) as T;

			if (instance != null) return instance;
			MemberInfo info = typeof(T);
			var attributes = info.GetCustomAttributes(true);
			foreach (var atribute in attributes)
			{
				var defineAttri = atribute as QMonoSingletonPath;
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
				if (!mIsUnitTestMode)
					Object.DontDestroyOnLoad(obj);
				instance = obj.AddComponent<T>();
			}

			instance.OnSingletonInit();

			return instance;
		}

		private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : MonoBehaviour
		{
			var obj = FindGameObject(null, path, true, dontDestroy);
			if (obj == null)
			{
				obj = new GameObject("Singleton of " + typeof(T).Name);
				if (dontDestroy && !mIsUnitTestMode)
				{
					Object.DontDestroyOnLoad(obj);
				}
			}

			return obj.AddComponent<T>();
		}

		static GameObject FindGameObject(GameObject root, string path, bool build, bool dontDestroy)
		{
			if (path == null || path.Length == 0)
			{
				return null;
			}

			string[] subPath = path.Split('/');
			if (subPath == null || subPath.Length == 0)
			{
				return null;
			}

			return FindGameObject(null, subPath, 0, build, dontDestroy);
		}

		static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy)
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

					if (dontDestroy && index == 0 && !mIsUnitTestMode)
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
}