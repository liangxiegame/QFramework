// Generate Id:2c4fcc50-8036-43a1-84a6-df63c102067b
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class GameOverUI
	{
		public const string NAME = "GameOverUI";

		public UnityEngine.UI.Text GameOverUIText;
		public UnityEngine.UI.Button RestartButton;
		public UnityEngine.UI.Button HomeButton;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new GameOverUI
			{
				transform = ilkitBehaviour.transform,
				gameObject = ilkitBehaviour.gameObject,
				MonoBehaviour = ilkitBehaviour
			};

			ilkitBehaviour.Script = ilBehaviour;

			ilBehaviour.SetupBinds();
			ilBehaviour.OnStart();

			ilkitBehaviour.OnDestroyAction += ilBehaviour.DestroyScript;
		}

		void SetupBinds()
		{
			GameOverUIText = transform.Find("GameOverUIText").GetComponent<UnityEngine.UI.Text>();
			RestartButton = transform.Find("RestartButton").GetComponent<UnityEngine.UI.Button>();
			HomeButton = transform.Find("HomeButton").GetComponent<UnityEngine.UI.Button>();
		}

		void ClearBinds()
		{
			GameOverUIText = null;
			RestartButton = null;
			HomeButton = null;
		}

		void DestroyScript()
		{
			OnDestroy();

			ClearBinds();

			transform = null;
			gameObject = null;
			MonoBehaviour = null;
		}
	}
}
