using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class AssetBundleResExample : MonoBehaviour
	{
		private ResLoader mResLoader = ResLoader.Allocate();

		public RawImage RawImage;

		private void Awake()
		{
			ResMgr.Init();
		}

		// Use this for initialization
		void Start()
		{
			RawImage rawImage = transform.Find("RawImage").GetComponent<RawImage>();

			RawImage.texture = mResLoader.LoadSync<Texture2D>("TestImage");
			
			// 通过下边方式也一样
			RawImage.texture = mResLoader.LoadSync<Texture2D>("testimage_png","TestImage");
		}

		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}