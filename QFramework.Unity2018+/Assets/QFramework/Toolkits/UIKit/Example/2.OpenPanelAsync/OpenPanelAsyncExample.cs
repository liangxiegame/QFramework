using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    public class OpenPanelAsyncExample : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return ResKit.InitAsync();

            yield return UIKit.OpenPanelAsync<UIBasicPanel>();
            
            // or
            // UIKit.OpenPanelAsync<UIBasicPanel>().ToAction().Start(this);
        }
    }
}
