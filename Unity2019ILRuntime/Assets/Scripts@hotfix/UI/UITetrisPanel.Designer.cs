// Generate Id:baecc316-2bbc-4e6e-bafa-81d4f3f92377
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class UITetrisPanel
	{
		public const string NAME = "UITetrisPanel";

		public UnityEngine.Camera MainCamera;
		public UnityEngine.RectTransform LogoName;
		public QFramework.ILKitBehaviour MenuUI;
		public QFramework.ILKitBehaviour GameUI;
		public QFramework.ILKitBehaviour SettingUI;
		public QFramework.ILKitBehaviour RankUI;
		public QFramework.ILKitBehaviour GameOverUI;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new UITetrisPanel
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
			MainCamera = transform.Find("MainCamera").GetComponent<UnityEngine.Camera>();
			LogoName = transform.Find("LogoName").GetComponent<UnityEngine.RectTransform>();
			MenuUI = transform.Find("MenuUI").GetComponent<QFramework.ILKitBehaviour>();
			GameUI = transform.Find("GameUI").GetComponent<QFramework.ILKitBehaviour>();
			SettingUI = transform.Find("SettingUI").GetComponent<QFramework.ILKitBehaviour>();
			RankUI = transform.Find("RankUI").GetComponent<QFramework.ILKitBehaviour>();
			GameOverUI = transform.Find("GameOverUI").GetComponent<QFramework.ILKitBehaviour>();
		}

		void ClearBinds()
		{
			MainCamera = null;
			LogoName = null;
			MenuUI = null;
			GameUI = null;
			SettingUI = null;
			RankUI = null;
			GameOverUI = null;
		}

		void DestroyScript()
		{
			OnDestroy();

			ClearBinds();

			transform = null;
			gameObject = null;
			MonoBehaviour = null;

			mPrivateData = null;
		}

		protected UITetrisPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITetrisPanelData());
			}
			set
			{
				mPrivateData = value;
			}
		}

		private UITetrisPanelData mPrivateData = null;
	}
}
