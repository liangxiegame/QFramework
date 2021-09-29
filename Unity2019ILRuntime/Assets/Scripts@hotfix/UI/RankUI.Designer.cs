// Generate Id:6e4579a8-b048-474c-a6e6-5993a5956a1e
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class RankUI
	{
		public const string NAME = "RankUI";

		public UnityEngine.UI.Button DestroyButton;
		public UnityEngine.UI.Text RankUIScoreText;
		public UnityEngine.UI.Text RankUIHighScoreText;
		public UnityEngine.UI.Text NumbersGameText;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new RankUI
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
			DestroyButton = transform.Find("DestroyButton").GetComponent<UnityEngine.UI.Button>();
			RankUIScoreText = transform.Find("ScoreLabel/RankUIScoreText").GetComponent<UnityEngine.UI.Text>();
			RankUIHighScoreText = transform.Find("HighScoreLabel/RankUIHighScoreText").GetComponent<UnityEngine.UI.Text>();
			NumbersGameText = transform.Find("NumbersGameLabel/NumbersGameText").GetComponent<UnityEngine.UI.Text>();
		}

		void ClearBinds()
		{
			DestroyButton = null;
			RankUIScoreText = null;
			RankUIHighScoreText = null;
			NumbersGameText = null;
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
