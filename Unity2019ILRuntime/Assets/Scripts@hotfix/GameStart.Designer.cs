// Generate Id:ff45482f-0055-41be-9f96-80edfaaf93b3
using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class GameStart
	{
		public const string NAME = "GameStart";

		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new GameStart
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
