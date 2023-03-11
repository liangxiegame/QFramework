using UnityEngine;

namespace QFramework.Example
{
    public class SequenceAndCallback : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Sequence Start:" + Time.time);

            ActionKit.Sequence()
                .Callback(() => Debug.Log("Delay Start:" + Time.time))
                .Delay(1.0f)
                .Callback(() => Debug.Log("Delay Finish:" + Time.time))
                .Start(this, _ => { Debug.Log("Sequence Finish:" + Time.time); });
        }
    }
}