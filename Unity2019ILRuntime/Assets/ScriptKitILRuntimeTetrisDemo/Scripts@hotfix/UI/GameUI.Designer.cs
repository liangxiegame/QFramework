// Generate Id:aed9e21b-ea65-4ed6-ada6-19514e36cd4f
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class GameUI
	{
		public const string NAME = "GameUI";

		public UnityEngine.UI.Button PauseButton;
		public UnityEngine.UI.Text ScoreText;
		public UnityEngine.UI.Text HighScoreText;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new GameUI
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
			PauseButton = transform.Find("PauseButton").GetComponent<UnityEngine.UI.Button>();
			ScoreText = transform.Find("ScoreLabel/ScoreText").GetComponent<UnityEngine.UI.Text>();
			HighScoreText = transform.Find("HighScoreLabel/HighScoreText").GetComponent<UnityEngine.UI.Text>();
		}

		void ClearBinds()
		{
			PauseButton = null;
			ScoreText = null;
			HighScoreText = null;
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
