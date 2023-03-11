using UnityEngine;

namespace QFramework.Example
{
	public class LoadSceneExample : MonoBehaviour
	{
		private ResLoader mResLoader = null;

		void Start()
		{
			ResKit.Init();

			mResLoader = ResLoader.Allocate();

			// 同步加载
			// mResLoader.LoadSceneSync("SceneRes");

			// 异步加载
			// mResLoader.LoadSceneAsync("SceneRes");

			// 异步加载
			mResLoader.LoadSceneAsync("SceneRes", onStartLoading: operation =>
			{
				// 做一些加载操作
				operation.completed += asyncOperation =>
				{
					Debug.Log("SceneRes loaded");
				};
			});
		}

		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}