using UnityEngine;

namespace QFramework.Example
{
    public class DelayFrameExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Delay Frame Start FrameCount:" + Time.frameCount);
            
            ActionKit.DelayFrame(1, () => { Debug.Log("Delay Frame Finish FrameCount:" + Time.frameCount); })
                .Start(this);


            ActionKit.Sequence()
                .DelayFrame(10)
                .Callback(() => Debug.Log("Sequence Delay FrameCount:" + Time.frameCount))
                .Start(this);

            // ActionKit.Sequence()
            //      .NextFrame()
            //      .Start(this);

            ActionKit.NextFrame(() => { }).Start(this);
        }
    }
}