// Generate Id:bda39d72-4ed2-4e7a-9e57-6c8ae21cb3e8
using UnityEngine;
using QFramework;

namespace QFramework
{
	public partial class ILUIPanelTester
	{
		public const string NAME = "ILUIPanelTester";

		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new ILUIPanelTester
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
		}

		void ClearBinds()
		{
		}

		void DestroyScript()
		{
			ClearBinds();

			transform = null;
			gameObject = null;
			MonoBehaviour = null;

			OnDestroy();
		}
	}
}
