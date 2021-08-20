/****************************************************************************
 * Copyright (c) 2018.3 ~ 7 liangxie
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

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 主要管理PoolManager
	/// </summary>
	public class GoManager : Singleton<GoManager>,IController
	{
		public delegate void PrefabLoadDoneCallback();

		private ResLoader mResLoader = ResLoader.Allocate();

		// 通过字符串拼接完成
		public const string PrefabPathHead = "Prefabs/Stage/";
		public const string Suffix         = ".prefab";

		public string PrefabPath = "Prefabs/Theme";

		private static int Theme
		{
			get { return PlatformRunner.Interface.GetModel<IGameModel>().CurTheme.Value; }
		}

		private readonly GameObject[] mSpawnGOs = new GameObject[5];

		private GoManager()
		{
		}

		public void LoadStagePool(int theme,GamePlayCtrl gamePlayCtrl,  PrefabLoadDoneCallback callback = null)
		{
			if (mSpawnGOs[theme] == null)
			{
				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
				watch.Start();
				mSpawnGOs[theme] = mResLoader.LoadSync<GameObject>("stage" + theme + "spawnpool").Instantiate();

				mSpawnGOs[theme].transform.parent = gamePlayCtrl.PoolManagerTrans;
				if (callback != null)
				{
					callback();
				}
			}
			else
			{
				Debug.LogWarning("theme:" + theme + " was loaded");
				if (callback != null)
				{
					callback();
				}
			}
		}

//		public SpawnPool GetThemeSpawnPool(int theme)
//		{
//			return PoolManager.Pools["Stage" + theme];
//		}

//		public Transform StageSpawn(string name)
//		{
//			return PoolManager.Pools["Stage" + Theme].Spawn(name);
//		}

		public void UnloadUnusedStagePool()
		{
			for (int i = 1; i <= 4; i++)
			{
				if (i == Theme)
				{

				}
//				else if (PoolManager.Pools.ContainsKey("Stage" + i))
//				{
//					PoolManager.Pools.Destroy("Stage" + i);
//				}
			}
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}