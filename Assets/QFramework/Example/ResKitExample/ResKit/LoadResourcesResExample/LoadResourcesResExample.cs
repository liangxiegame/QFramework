using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class LoadResourcesResExample : MonoBehaviour
	{
		private RawImage mRawImage;
		
		ResLoader mResLoader = ResLoader.Allocate();
	
		
		// Use this for initialization
		void Start()
		{
			mRawImage = transform.Find("RawImage").GetComponent<RawImage>();

			mRawImage.texture = mResLoader.LoadSync<Texture2D>("resources://TestTexture");
		}

		private void OnDestroy()
		{
			Log.I("On Destroy ");
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}