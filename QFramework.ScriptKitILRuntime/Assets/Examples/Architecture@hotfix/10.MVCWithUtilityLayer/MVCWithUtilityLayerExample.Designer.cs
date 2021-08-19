// Generate Id:1243221d-d02f-4565-97d1-8ddca923788b
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class MVCWithUtilityLayerExample
	{
		public const string NAME = "MVCWithUtilityLayerExample";

		public UnityEngine.UI.Text Text;
		public UnityEngine.UI.Button Button;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new MVCWithUtilityLayerExample
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
		}

		void ClearBinds()
		{
			Text = null;
			Button = null;
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
