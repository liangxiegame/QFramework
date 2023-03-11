using QFramework;
using UnityEngine;

namespace ShootingEditor2D
{
    public class AttackPlayer : ShootingEditor2DController
    {
        public int Hurt = 1;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                this.SendCommand(new HurtPlayerCommand(Hurt));
            }
        }
    }
}
