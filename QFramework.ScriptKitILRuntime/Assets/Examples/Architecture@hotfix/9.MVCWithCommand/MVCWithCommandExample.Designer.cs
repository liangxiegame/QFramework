// Generate Id:d6b525bc-103e-4945-9b5a-6c1f75df7baf
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class MVCWithCommandExample
	{
		public const string NAME = "MVCWithCommandExample";

		public UnityEngine.UI.Text Text;
		public UnityEngine.UI.Button Button;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new MVCWithCommandExample
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
