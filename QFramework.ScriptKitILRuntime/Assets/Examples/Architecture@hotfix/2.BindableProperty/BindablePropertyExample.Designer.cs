// Generate Id:866ccbcd-1472-4fd9-994a-5646a80663a4
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class BindablePropertyExample
	{
		public const string NAME = "BindablePropertyExample";

		public UnityEngine.UI.Button Button;
		public UnityEngine.UI.Text Text;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new BindablePropertyExample
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
			Button = transform.Find("Button").GetComponent<UnityEngine.UI.Button>();
			Text = transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
		}

		void ClearBinds()
		{
			Button = null;
			Text = null;
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
