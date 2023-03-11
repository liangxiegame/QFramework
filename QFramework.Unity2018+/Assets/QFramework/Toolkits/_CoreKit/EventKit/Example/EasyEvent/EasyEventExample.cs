using UnityEngine;

namespace QFramework.Example
{
    public class EasyEventExample : MonoBehaviour
    {
        private EasyEvent<string> mSomeStringEvent = new EasyEvent<string>();

        private void Awake()
        {
            mSomeStringEvent.Register(str =>
            {
                Debug.Log(str);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            EasyEvents.Register<EasyEvent<int>>();
            EasyEvents.Register<MyEvent>();
            
            EasyEvents.Get<EasyEvent<int>>().Register(number =>
            {
                Debug.Log(number);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            EasyEvents.Get<MyEvent>().Register((str1,str2)=>
            {
                Debug.Log(str1 +":" + str2);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mSomeStringEvent.Trigger("Hello World");
            }

            if (Input.GetMouseButtonDown(1))
            {
                EasyEvents.Get<EasyEvent<int>>().Trigger(123);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EasyEvents.Get<MyEvent>().Trigger("你好","EasyEvent");
            }
        }
        
        public class MyEvent : EasyEvent<string,string>
        {
            
        }
    }
}