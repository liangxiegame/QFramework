using UnityEngine;

namespace QFramework.Example
{
    public class Enemy : QMonoBehaviour
    {
        public override IManager Manager
        {
            get { return EnemyManager.Instance; }
        }

        public void PlaySkill(string skillName)
        {
            Debug.Log(this.name + ":" + skillName);
        }
    }
}