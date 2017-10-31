#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using UnityEngine;
 // for enable gameObject.EventAsObservbale()

namespace QFramework.Examples
{
    public class Sample03_GameObjectAsObservable : MonoBehaviour
    {
        void Start()
        {
            // All events can subscribe by ***AsObservable if enables QFramework
            this.OnMouseDownAsObservable()
                .SelectMany(_ => this.gameObject.UpdateAsObservable())
                .TakeUntil(this.gameObject.OnMouseUpAsObservable())
                .Select(_ => Input.mousePosition)
                .RepeatUntilDestroy(this)
                .Subscribe(x => Debug.Log(x), ()=> Debug.Log("!!!" + "complete"));
        }
    }
}

#endif