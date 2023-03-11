using UnityEngine;

namespace QFramework.Example
{
    public class ParallelExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Parallel Start:" + Time.time);

            ActionKit.Parallel()
                .Delay(1.0f, () => { Debug.Log(Time.time); })
                .Delay(2.0f, () => { Debug.Log(Time.time); })
                .Delay(3.0f, () => { Debug.Log(Time.time); })
                .Start(this, () =>
                {
                    Debug.Log("Parallel Finish:" + Time.time);
                });
        }
    }
}