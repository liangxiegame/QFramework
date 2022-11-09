using System;
using UnityEngine;

namespace QFramework.Example
{
    public class UIKitStackExample : MonoBehaviour
    {
        private void Start()
        {
            ResKit.Init();
            
            UIKit.Stack.Push(UIKit.OpenPanel<UIStackPanel1>());
            UIKit.Stack.Push(UIKit.OpenPanel<UIStackPanel2>());
            
            UIKit.Stack.Pop();


            ActionKit.Delay(3, () =>
            {
                UIKit.Stack.Pop();

            }).Start(this);
        }
    }
}
