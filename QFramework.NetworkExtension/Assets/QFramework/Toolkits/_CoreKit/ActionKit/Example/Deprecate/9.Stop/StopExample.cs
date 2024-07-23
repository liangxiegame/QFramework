using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    public class StopExample : MonoBehaviour
    {
        IEnumerator Start()
        {
            var delayAction = this.Delay(5, () =>
            {
                Debug.Log("Delay Action");
            });

            yield return new WaitForSeconds(2);

            if (!delayAction.Finished)
            {
                // 停止掉
                delayAction.Dispose();
                Debug.Log("暂停掉了:" + Time.time);
            }
        }
    }
}
