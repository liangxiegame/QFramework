// Generate Id:27f14c4b-0466-4931-ad92-198848adcea4
using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class TimeSystemViewController
	{
		public const string NAME = "TimeSystemViewController";

		public UnityEngine.UI.Button BtnCreateA;
		public UnityEngine.UI.Button BtnCreateB;
		public UnityEngine.UI.Button BtnCreateC;
		public UnityEngine.RectTransform ItemRoot;
		public QFramework.ILKitBehaviour UITimeTickTaskItem;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new TimeSystemViewController
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
			BtnCreateA = transform.Find("BtnCreateA").GetComponent<UnityEngine.UI.Button>();
			BtnCreateB = transform.Find("BtnCreateB").GetComponent<UnityEngine.UI.Button>();
			BtnCreateC = transform.Find("BtnCreateC").GetComponent<UnityEngine.UI.Button>();
			ItemRoot = transform.Find("Scroll View/Viewport/ItemRoot").GetComponent<UnityEngine.RectTransform>();
			UITimeTickTaskItem = transform.Find("UITimeTickTaskItem").GetComponent<QFramework.ILKitBehaviour>();
		}

		void ClearBinds()
		{
			BtnCreateA = null;
			BtnCreateB = null;
			BtnCreateC = null;
			ItemRoot = null;
			UITimeTickTaskItem = null;
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
