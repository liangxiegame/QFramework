// Generate Id:89f42c56-1b0d-4ac1-b631-790bbc94ac19
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class UITimeTickTaskItem
	{
		public const string NAME = "UITimeTickTaskItem";

		public UnityEngine.UI.Text Text;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new UITimeTickTaskItem
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
