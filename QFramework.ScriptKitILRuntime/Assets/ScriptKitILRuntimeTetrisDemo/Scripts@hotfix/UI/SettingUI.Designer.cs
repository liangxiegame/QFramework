// Generate Id:8af6847a-9111-4f79-9a97-e0e6c71ea0ea
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class SettingUI
	{
		public const string NAME = "SettingUI";

		public UnityEngine.UI.Button AudioButton;
		public UnityEngine.UI.Image Mute;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new SettingUI
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
			AudioButton = transform.Find("AudioButton").GetComponent<UnityEngine.UI.Button>();
			Mute = transform.Find("AudioButton/Mute").GetComponent<UnityEngine.UI.Image>();
		}

		void ClearBinds()
		{
			AudioButton = null;
			Mute = null;
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
