using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class TypeEventSystemUnRegisterExample : MonoBehaviour
    {

        public struct EventA
        {
            
        }
        
        private void Start()
        {
            TypeEventSystem.Global.Register<EventA>(OnEventA);
        }

        void OnEventA(EventA e)
        {
            
        }

        private void OnDestroy()
        {
            TypeEventSystem.Global.UnRegister<EventA>(OnEventA);
        }
        
        
        public class NoneMonoScript : IUnRegisterList
        {
            public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();


            void Start()
            {
                TypeEventSystem.Global.Register<EasyEventExample.EventA>(a =>
                {
                    
                }).AddToUnregisterList(this);
            }

            void OnDestroy()
            {
                this.UnRegisterAll();
            }
        }
    }
}