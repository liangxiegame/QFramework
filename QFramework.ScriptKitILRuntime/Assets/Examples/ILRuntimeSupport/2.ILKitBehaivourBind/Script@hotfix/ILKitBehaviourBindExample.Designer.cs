// Generate Id:e63f4d4f-98e7-4d38-b61f-ae6872fec145
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class ILKitBehaviourBindExample
	{
		public const string NAME = "ILKitBehaviourBindExample";

		public UnityEngine.UI.Button Button;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new ILKitBehaviourBindExample
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
			Button = transform.Find("Canvas/Button").GetComponent<UnityEngine.UI.Button>();
		}

		void ClearBinds()
		{
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
