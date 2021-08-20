using UnityEngine;

namespace QFramework.PlatformRunner
{
    public class Main : MonoBehaviour
    {
        private void Start()
        {
            ResKit.Init();

            UIKit.Root.SetResolution(1136, 640, 1);
            UIKit.OpenPanel<UIHomePanel>();
        }
    }
}