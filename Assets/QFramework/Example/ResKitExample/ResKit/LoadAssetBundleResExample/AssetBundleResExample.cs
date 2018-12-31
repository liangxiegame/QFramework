using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class AssetBundleResExample : MonoBehaviour
	{

		ResLoader mResLoader = ResLoader.Allocate();


		private void Awake()
		{
			ResMgr.Init();
		}

		// Use this for initialization
		void Start()
		{
			RawImage rawImage = transform.Find("RawImage").GetComponent<RawImage>();

			rawImage.texture = mResLoader.LoadSync<Texture2D>("TestImage");
			
//			rawImage.texture = mResLoader.LoadSync<Texture2D>("testimage_png","TestImage");
		}

		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}