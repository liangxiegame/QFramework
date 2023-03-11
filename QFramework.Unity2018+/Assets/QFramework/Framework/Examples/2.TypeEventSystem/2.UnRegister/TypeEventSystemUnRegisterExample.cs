using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class TypeEventSystemUnRegisterExample : MonoBehaviour
    {

        public struct EventA
        {
            
        }

        public struct EventB
        {
            
        }
        
        private void Start()
        {
            TypeEventSystem.Global.Register<EventA>(OnEventA);
            TypeEventSystem.Global.Register<EventB>(b => { }).UnRegisterWhenGameObjectDestroyed(this);
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
                TypeEventSystem.Global.Register<EventA>(a =>
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