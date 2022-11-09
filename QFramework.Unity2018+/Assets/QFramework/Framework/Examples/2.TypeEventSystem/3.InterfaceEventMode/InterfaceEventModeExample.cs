using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public struct InterfaceEventA
    {
            
    }

    public struct InterfaceEventB
    {
        
    }

    public class InterfaceEventModeExample : MonoBehaviour
        , IOnEvent<InterfaceEventA>
        , IOnEvent<InterfaceEventB>
    {
        public void OnEvent(InterfaceEventA e)
        {
            Debug.Log(e.GetType().Name);
        }
        
        public void OnEvent(InterfaceEventB e)
        {
            Debug.Log(e.GetType().Name);
        }

        private void Start()
        {
            this.RegisterEvent<InterfaceEventA>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<InterfaceEventB>();
        }

        private void OnDestroy()
        {
            this.UnRegisterEvent<InterfaceEventB>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send<InterfaceEventA>();
                TypeEventSystem.Global.Send<InterfaceEventB>();
            }
        }
    }
}
