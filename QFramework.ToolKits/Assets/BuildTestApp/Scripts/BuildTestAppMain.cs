using System.Collections;
using QFramework;
using QFramework.Example;
using UnityEngine;

namespace BuildTestApp
{
    public class BuildTestAppMain : MonoBehaviour
    {
        void Start()
        {
            ActionKit.Sequence()
                .Append(ResKit.InitAsync().ToAction())
                .Append(UIKit.OpenPanelAsync<UIStartPanel>().ToAction())
                .Start(this);
            
            
            // Or
            // yield return ResKit.InitAsync();
            //
            // yield return UIKit.OpenPanelAsync<UIStartPanel>();
            //
            // UIKit.GetPanel<UIStartPanel>();
        }
    }
}