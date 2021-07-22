using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework.Example
{
	public class LoadSceneExample : MonoBehaviour
	{
		private ResLoader mResLoader = null;
		// Use this for initialization
		void Start()
		{
			ResKit.Init();

			mResLoader = ResLoader.Allocate();

			
			Async();
		}

		/// <summary>
		/// 同步的加载方式
		/// </summary>
		void Sync()
		{
			mResLoader.LoadSync("SceneRes");

			SceneManager.LoadScene("SceneRes");
		}

		/// <summary>
		/// 异步的加载方式
		/// </summary>
		void Async()
		{			
			mResLoader.Add2Load("SceneRes");

			mResLoader.LoadAsync(() =>
			{
				SceneManager.LoadScene("SceneRes");
				
			});
		}


		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}