// Generate Id:71af5981-1dba-4473-a253-5060bbd7ebb2
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class MenuUI
	{
		public const string NAME = "MenuUI";

		public UnityEngine.UI.Button StartButton;
		public UnityEngine.UI.Button MenuRestartButton;
		public UnityEngine.UI.Button SettingButton;
		public UnityEngine.UI.Button RankButton;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new MenuUI
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
			StartButton = transform.Find("StartButton").GetComponent<UnityEngine.UI.Button>();
			MenuRestartButton = transform.Find("MenuRestartButton").GetComponent<UnityEngine.UI.Button>();
			SettingButton = transform.Find("SettingButton").GetComponent<UnityEngine.UI.Button>();
			RankButton = transform.Find("RankButton").GetComponent<UnityEngine.UI.Button>();
		}

		void ClearBinds()
		{
			StartButton = null;
			MenuRestartButton = null;
			SettingButton = null;
			RankButton = null;
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
