using UnityEngine;

namespace QFramework.Example
{
    public class EventTriggerExample : MonoBehaviour
    {
        void Start()
        {
            GameObject.Find("Ground").OnTriggerEnter2DEvent(collider2D1 =>
            {
                Debug.Log(collider2D1.name + ": entered");
                
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            var uiImage = GameObject.Find("Canvas/Image");

            uiImage.OnPointerDownEvent(data =>
            {
                Debug.Log("Click");
                
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}
