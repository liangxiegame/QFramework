using UnityEngine;

namespace QFramework.Example
{
	public class ManagerOfManagersExample : QMonoBehaviour
	{
		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				// 可以直接用 Manager 去发送消息
				SendMsg(new EnemySkillPlay()
				{
					SkillName = "AOE",
					EnemyId = "123"
				});
			}
		}

		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}
	}
}