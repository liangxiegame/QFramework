using UnityEngine;

namespace QFramework.Example
{
    public class ConditionExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Sequence()
                .Callback(() => Debug.Log("Before Condition"))
                .Condition(() => Input.GetMouseButtonDown(0))
                .Callback(() => Debug.Log("Mouse Clicked"))
                .Start(this);
        }
    }
}