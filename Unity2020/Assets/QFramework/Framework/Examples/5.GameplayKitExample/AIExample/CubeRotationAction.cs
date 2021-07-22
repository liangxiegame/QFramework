using UnityEngine;

namespace QFramework.Example
{
    public class CubeRotationAction : AIAction
    {
        public override void PerformAction()
        {
            mActive = true;

        }

        private bool mActive;

        public override void OnEnterState()
        {
            
        }

        public override void OnExitState()
        {
            mActive = false;
        }

        private void Update()
        {
            if (mActive)
            {
                this.transform.Rotate( 0, 10, 0);
            }
        }
    }
}