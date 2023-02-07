using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class CubeLerpExample : MonoBehaviour
    {
        void Start()
        {
            ActionKit.Lerp(0, 360, 5.0f, (v) =>
            {
                this.Rotation(Quaternion.Euler(0, 0, v));
            }).Start(this);

        }

        void Update()
        {

        }
    }
}
