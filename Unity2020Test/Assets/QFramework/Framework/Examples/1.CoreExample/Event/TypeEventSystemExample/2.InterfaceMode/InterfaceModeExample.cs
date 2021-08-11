using UnityEngine;

namespace QFramework.Example
{
    // 时间定义（结构体有更好的性能，更少的 gc)
    public struct OnLeftMouseClickEvent
    {
    }

    public struct OnRightMouseClickEvent
    {
    }

    public class InterfaceModeExample : MonoBehaviour,
        IOnEvent<OnLeftMouseClickEvent>,
        IOnEvent<OnRightMouseClickEvent>
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send(new OnLeftMouseClickEvent());
            }

            if (Input.GetMouseButtonDown(1))
            {
                TypeEventSystem.Global.Send(new OnRightMouseClickEvent());
            }
        }

        private void Start()
        {
            this.RegisterEvent<OnLeftMouseClickEvent>().UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnRightMouseClickEvent>().UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void OnEvent(OnLeftMouseClickEvent e)
        {
            Debug.Log("Left Mouse Clicked");
        }

        public void OnEvent(OnRightMouseClickEvent e)
        {
            Debug.Log("Right Mouse Clicked");
        }
        
        /* 第二种注册方式
        OnEnable 注册 OnDisable 注销
        private void OnEnable()
        {
            this.RegisterEvent<OnLeftMouseClickEvent>().DisposeWhenGameObjectDisabled(this);
            this.RegisterEvent<OnRightMouseClickEvent>().DisposeWhenGameObjectDisabled(this);
        }
        */


        /* 第三种注册方式
        OnEnable 注册 OnDisable 注销
        private void OnEnable()
        {
            this.RegisterEvent<OnLeftMouseClickEvent>();
            this.RegisterEvent<OnRightMouseClickEvent>();
        }
        
        private void OnDisable()
        {
            this.UnRegisterEvent<OnLeftMouseClickEvent>();
            this.UnRegisterEvent<OnRightMouseClickEvent>();
        }
*/
     
    }
}