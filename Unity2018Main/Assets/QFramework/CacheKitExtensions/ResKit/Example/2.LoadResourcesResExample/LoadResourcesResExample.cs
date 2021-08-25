using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class LoadResourcesResExample : MonoBehaviour
	{
		public RawImage RawImage;
		
		private ResLoader mResLoader = ResLoader.Allocate();
		
		// Use this for initialization
		private void Start()
		{
			RawImage.texture = mResLoader.LoadSync<Texture2D>("resources://TestTexture");
		}

		private void OnDestroy()
		{
			Log.I("On Destroy ");
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}