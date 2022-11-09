using UnityEngine;

namespace QFramework.Example
{
    public class DelayExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Start Time:" + Time.time);
            
            ActionKit.Delay(1.0f, () =>
            {
                Debug.Log("End Time:" + Time.time);
                
            }).Start(this);
        }
    }
}