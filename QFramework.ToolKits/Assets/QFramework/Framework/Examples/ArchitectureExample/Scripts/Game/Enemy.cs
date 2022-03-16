using UnityEngine;

namespace QFramework.PointGame
{
    public class Enemy : MonoBehaviour,IController
    {
        
        private void OnMouseDown()
        {
            gameObject.SetActive(false);
            
            this.SendCommand<KillEnemyCommand>();
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}
