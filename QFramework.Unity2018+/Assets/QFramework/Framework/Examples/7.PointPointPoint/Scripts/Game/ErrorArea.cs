using UnityEngine;

namespace QFramework.PointGame
{
    public class ErrorArea : MonoBehaviour,IController
    {
        private void OnMouseDown()
        {
            this.SendCommand<MissCommand>();
        }

        public IArchitecture GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}
