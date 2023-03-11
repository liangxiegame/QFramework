using UnityEngine;

namespace QFramework.Example
{
    public class TypeEventSystemInheritEventExample : MonoBehaviour
    {
        public interface IEventA
        {
            
        }
        
        public struct EventB : IEventA
        {
            
        }

        private void Start()
        {
            TypeEventSystem.Global.Register<IEventA>(e =>
            {
                Debug.Log(e.GetType().Name);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send<IEventA>(new EventB());
                
                // 无效
                TypeEventSystem.Global.Send<EventB>();
            }
        }
    }
}
