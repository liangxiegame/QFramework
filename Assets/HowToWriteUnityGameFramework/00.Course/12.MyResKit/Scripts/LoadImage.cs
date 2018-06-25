using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace MyResKit
{
	public class LoadImage : MonoBehaviour
	{
		private ResLoader mLoader = new ResLoader();

		// Use this for initialization
		IEnumerator Start()
		{
			GetComponent<RawImage>().texture = mLoader.Load<Texture2D>("Square");

			yield return new WaitForSeconds(3.0f);

			GetComponent<RawImage>().texture = mLoader.Load<Texture2D>("SquareA");

			yield return new WaitForSeconds(3.0f);

			GetComponent<RawImage>().texture = mLoader.Load<Texture2D>("SquareB");

			yield return new WaitForSeconds(3.0f);

			mLoader.UnloadAll();
		}

		void OnDestroy()
		{
			mLoader = null;
		}
	}

	public class ResLoader
	{
		/// <summary>
		/// 共享的 
		/// </summary>
		private static List<Res> mSharedLoadedReses = new List<Res>();
		
		
		/// <summary>
		/// 持有的
		/// </summary>
		private List<Res> mResList = new List<Res>();

		public T Load<T>(string assetName) where T : Object
		{
			var loadedRes = mResList.Find(loadedAsset=>loadedAsset.Name == assetName);
			
			if (loadedRes != null)
			{
				return loadedRes as T;
			}

			loadedRes = mSharedLoadedReses.Find(loadedAsset => loadedAsset.Name == assetName);

			if (loadedRes != null)
			{
				loadedRes.Retain();
				
				mResList.Add(loadedRes);
				
				return loadedRes as T;
			}
			
			var asset = Resources.Load<T>(assetName);

			var res = new Res(asset);

			mSharedLoadedReses.Add(res);
			
			loadedRes.Retain();

			mResList.Add(loadedRes);

			return asset;
		}

		public void UnloadAll()
		{
			foreach (var asset in mResList)
			{
				asset.Release();
			}

			mResList.Clear();
			mResList = null;
		}
	}

	public class Res
	{
		public string Name
		{
			get { return mAsset.name; }
		}
		
		public Res(Object asset)
		{
			mAsset = asset;
		}
		
		private Object mAsset;
		
		private int mReferenceCount = 0;

		public void Retain()
		{
			mReferenceCount++;
		}

		public void Release()
		{
			mReferenceCount--;

			if (mReferenceCount == 0)
			{
				Resources.UnloadAsset(mAsset);
			}
		}
	}

	public class OtherPage : MonoBehaviour
	{

	}
}