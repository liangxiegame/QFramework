using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    public class CoroutineExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Coroutine(SomeCoroutine).Start(this);
            
            SomeCoroutine().ToAction().Start(this);

            ActionKit.Sequence()
                .Coroutine(SomeCoroutine)
                .Start(this);
        }

        IEnumerator SomeCoroutine()
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Hello:" + Time.time);
        }
    }
}