using UnityEngine;

namespace QFramework.Example
{
    public class InitAndCallApisExample : MonoBehaviour
    {
        private void Start()
        {
            // UIKit 的资源管理默认使用的是 ResKit
            ResKit.Init();

            // UIKit 的分辨率设置
            UIKit.Root.SetResolution(1920, 1080, 1);

            // 打开 UIBasicPanel
            UIKit.OpenPanel<UIBasicPanel>();


            ActionKit.Sequence()
                .Delay(1f)
                .Callback(() => UIKit.HidePanel<UIBasicPanel>())
                .Delay(1)
                .Callback(() => UIKit.OpenPanel<UIBasicPanel>())
                .Delay(1)
                .Callback(() => UIKit.HidePanel<UIBasicPanel>())
                .Delay(1)
                .Callback(() => UIKit.ShowPanel<UIBasicPanel>())
                .Start(this);
        }
    }
}