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

using QF.Extensions;

namespace QF.Res
{
	using UnityEngine;
	using System.Collections;

	public class AssetRes : Res
	{
		protected string[]           mAssetBundleArray;
		protected AssetBundleRequest mAssetBundleRequest;

		public static AssetRes Allocate(string name, string onwerBundleName = null)
		{
			AssetRes res = SafeObjectPool<AssetRes>.Instance.Allocate();
			if (res != null)
			{
				res.AssetName = name;
				res.mOwnerBundleName = onwerBundleName;
				res.InitAssetBundleName();
			}

			return res;
		}

		protected string AssetBundleName
		{
			get { return mAssetBundleArray == null ? null : mAssetBundleArray[0]; }
		}

		public AssetRes(string assetName) : base(assetName)
		{

		}

		public AssetRes()
		{

		}

		public override void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
		{
			strategy.OnSyncLoadFinish(loader, this);
		}

		public override void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
		{
			strategy.OnAsyncLoadFinish(loader, this);
		}

		public override bool LoadSync()
		{
			if (!CheckLoadAble())
			{
				return false;
			}

			if (string.IsNullOrEmpty(AssetBundleName))
			{
				return false;
			}


			Object obj = null;

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor && !string.Equals(mAssetName, "assetbundlemanifest"))
			{
				var abR = ResMgr.Instance.GetRes<AssetBundleRes>(AssetBundleName);

				var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
				if (assetPaths.Length == 0)
				{
					Log.E("Failed Load Asset:" + mAssetName);
					OnResLoadFaild();
					return false;
				}
				
				HoldDependRes();

				State = ResState.Loading;

				obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);
			}
			else
#endif
			{
				var abR = ResMgr.Instance.GetRes<AssetBundleRes>(AssetBundleName);

				if (abR == null || abR.AssetBundle == null)
				{
					Log.E("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
					return false;
				}
				
				HoldDependRes();

				State = ResState.Loading;
				
				obj = abR.AssetBundle.LoadAsset(mAssetName);
			}

			UnHoldDependRes();

			if (obj == null)
			{
				Log.E("Failed Load Asset:" + mAssetName);
				OnResLoadFaild();
				return false;
			}

			mAsset = obj;

			State = ResState.Ready;
			return true;
		}

		public override void LoadAsync()
		{
			if (!CheckLoadAble())
			{
				return;
			}

			if (string.IsNullOrEmpty(AssetBundleName))
			{
				return;
			}

			State = ResState.Loading;

			ResMgr.Instance.PushIEnumeratorTask(this);
		}

		public override IEnumerator DoLoadAsync(System.Action finishCallback)
		{
			if (RefCount <= 0)
			{
				OnResLoadFaild();
				finishCallback();
				yield break;
			}

			
            //Object obj = null;

			var abR = ResMgr.Instance.GetRes<AssetBundleRes>(AssetBundleName);

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor && !string.Equals(mAssetName, "assetbundlemanifest"))
			{
				var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
				if (assetPaths.Length == 0)
				{
					Log.E("Failed Load Asset:" + mAssetName);
					OnResLoadFaild();
					finishCallback();
					yield break;
				}

				//确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
				HoldDependRes();

				mAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);

				UnHoldDependRes();
			}
			else
#endif
			{
				
				if (abR == null || abR.AssetBundle == null)
				{
					Log.E("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
					OnResLoadFaild();
					finishCallback();
					yield break;
				}
				
				
				HoldDependRes();

				State = ResState.Loading;
				
				var abQ = abR.AssetBundle.LoadAssetAsync(mAssetName);
				mAssetBundleRequest = abQ;

				yield return abQ;

				mAssetBundleRequest = null;

				UnHoldDependRes();

				if (!abQ.isDone)
				{
					Log.E("Failed Load Asset:" + mAssetName);
					OnResLoadFaild();
					finishCallback();
					yield break;
				}

				mAsset = abQ.asset;
			}

			State = ResState.Ready;

			finishCallback();
		}

		public override string[] GetDependResList()
		{
			return mAssetBundleArray;
		}

		public override void OnRecycled()
		{
			mAssetBundleArray = null;
		}

		public override void Recycle2Cache()
		{
			SafeObjectPool<AssetRes>.Instance.Recycle(this);
		}

		protected override float CalculateProgress()
		{
			if (mAssetBundleRequest == null)
			{
				return 0;
			}

			return mAssetBundleRequest.progress;
		}

		protected void InitAssetBundleName()
		{
			mAssetBundleArray = null;

			var resSearchRule = ResSearchRule.Allocate(mAssetName, mOwnerBundleName);
			var config = ResDatas.Instance.GetAssetData(resSearchRule);
			resSearchRule.Recycle2Cache();

			if (config == null)
			{
				Log.E("Not Find AssetData For Asset:" + mAssetName);
				return;
			}

			var assetBundleName =
				ResDatas.Instance.GetAssetBundleName(config.AssetName, config.AssetBundleIndex, mOwnerBundleName);

			if (string.IsNullOrEmpty(assetBundleName))
			{
				Log.E("Not Find AssetBundle In Config:" + config.AssetBundleIndex + mOwnerBundleName);
				return;
			}

			mAssetBundleArray = new string[1];
			mAssetBundleArray[0] = assetBundleName;
		}

		public override string ToString()
		{
			return "Type:Asset\t {0}\t FromAssetBundle:{1}".FillFormat(base.ToString(), AssetBundleName);
		}
	}
}