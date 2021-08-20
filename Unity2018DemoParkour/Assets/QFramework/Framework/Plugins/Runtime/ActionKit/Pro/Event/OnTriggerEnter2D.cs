using System;
using UnityEngine;

namespace QFramework
{
    public class OnTriggerEnter2D : ActionKitEvent
    {
        private void OnTriggerEnter(Collider other)
        {
            Execute();
        }
    }
}