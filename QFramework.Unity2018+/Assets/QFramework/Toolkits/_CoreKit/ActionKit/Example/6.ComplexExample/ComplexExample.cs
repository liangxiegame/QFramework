using UnityEngine;

namespace QFramework.Example
{
    public class ComplexExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Sequence()
                .Callback(() => Debug.Log("Sequence Start"))
                .Callback(() => Debug.Log("Parallel Start"))
                .Parallel(p =>
                {
                    p.Delay(1.0f, () => Debug.Log("Delay 1s Finished"))
                        .Delay(2.0f, () => Debug.Log("Delay 2s Finished"));
                })
                .Callback(() => Debug.Log("Parallel Finished"))
                .Callback(() => Debug.Log("Check Mouse Clicked"))
                .Sequence(s =>
                {
                    s.Condition(() => Input.GetMouseButton(0))
                        .Callback(() => Debug.Log("Mouse Clicked"));
                })
                .Start(this, () =>
                {
                    Debug.Log("Finish");
                    
                });
        }
    }
}