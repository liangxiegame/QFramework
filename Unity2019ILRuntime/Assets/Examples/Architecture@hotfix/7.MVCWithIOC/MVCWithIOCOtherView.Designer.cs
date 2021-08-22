// Generate Id:941dc1ac-7b5c-408d-9f37-5e4ab188065d
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class MVCWithIOCOtherView
	{
		public const string NAME = "MVCWithIOCOtherView";

		public UnityEngine.UI.Text Text;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new MVCWithIOCOtherView
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
		}

		void ClearBinds()
		{
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
