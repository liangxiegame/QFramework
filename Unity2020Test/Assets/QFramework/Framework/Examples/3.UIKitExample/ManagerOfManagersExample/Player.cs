using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{

	public enum PlayerEvent
	{
		Start = QMgrID.Game,
		Run,
		End
	}
	
	public class Player : QMonoBehaviour
	{
		public class GameManager : QMgrBehaviour,ISingleton
		{
			public override int ManagerId
			{
				get { return QMgrID.Game; }
			}

			public void OnSingletonInit()
			{
				
			}

			public static GameManager Instance
			{
				get { return MonoSingletonProperty<GameManager>.Instance; }
			}
		}
		
		private IManager mManager;

		// Use this for initialization
		void Start()
		{
			RegisterEvent(PlayerEvent.Run);
		}

		// Update is called once per frame

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			switch (eventId)
			{
				case (int)PlayerEvent.Run:
					
					Log.I("收到跑的消息了");
					break;
			}
		}

		public override IManager Manager
		{
			get { return GameManager.Instance ; }
		}
	}
	
	
}