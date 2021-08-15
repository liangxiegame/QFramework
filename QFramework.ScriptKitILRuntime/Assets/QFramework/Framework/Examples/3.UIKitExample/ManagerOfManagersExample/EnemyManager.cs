
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
	[QFramework.MonoSingletonPath("[Game]/EnemyManager")]
	public class EnemyManager : QMgrBehaviour, ISingleton
	{
		public override int ManagerId
		{
			get { return MgrID.Enemy; }
		}

		void ISingleton.OnSingletonInit()
		{

		}

		public static EnemyManager Instance
		{
			get { return MonoSingletonProperty<EnemyManager>.Instance; }
		}


		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			if (eventId == (int) EnemyEvent.SkillEvent.Play)
			{
				var enemySkillPlay = msg as EnemySkillPlay;

				var enemy = mEnemies[enemySkillPlay.EnemyId];

				enemy.PlaySkill(enemySkillPlay.SkillName);
				
				Debug.Log(enemySkillPlay.EnemyId + ":" + enemySkillPlay.SkillName);
			}
		}
		
		
		private Dictionary<string,Enemy> mEnemies = new Dictionary<string, Enemy>();

		private void Awake()
		{
			CreateEnemy("123");
			CreateEnemy("456");
			CreateEnemy("789");
		}

		void CreateEnemy(string enemyId)
		{
			var enemyObj = new GameObject("Enemy" + enemyId);
			var enemyScript = enemyObj.AddComponent<Enemy>();

			mEnemies.Add(enemyId, enemyScript);
		}
	}
}