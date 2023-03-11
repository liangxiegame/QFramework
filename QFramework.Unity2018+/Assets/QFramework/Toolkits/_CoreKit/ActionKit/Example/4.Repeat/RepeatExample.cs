using UnityEngine;

namespace QFramework.Example
{
    public class RepeatExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Repeat()
                .Condition(() => Input.GetMouseButtonDown(0))
                .Callback(() => Debug.Log("Mouse Clicked"))
                .Start(this);


            ActionKit.Repeat(5)
                .Condition(() => Input.GetMouseButtonDown(1))
                .Callback(() => Debug.Log("Mouse right clicked"))
                .Start(this, () =>
                {
                    Debug.Log("Right click finished");
                });
        }
    }
}