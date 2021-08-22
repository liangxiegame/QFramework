// Generate Id:fe8d8b60-13e4-45f2-80d9-b80701dc4c5d
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class MVCWithSystemLayerExample
	{
		public const string NAME = "MVCWithSystemLayerExample";

		public UnityEngine.UI.Text Text;
		public UnityEngine.UI.Button Button;
		public UnityEngine.UI.Text MissionText;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new MVCWithSystemLayerExample
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
			Text = transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
			Button = transform.Find("Button").GetComponent<UnityEngine.UI.Button>();
			MissionText = transform.Find("MissionText").GetComponent<UnityEngine.UI.Text>();
		}

		void ClearBinds()
		{
			Text = null;
			Button = null;
			MissionText = null;
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
