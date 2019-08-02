/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

namespace QF
{
	using System.Reflection;
	using UnityEngine;
	
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

		private static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy)
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
}