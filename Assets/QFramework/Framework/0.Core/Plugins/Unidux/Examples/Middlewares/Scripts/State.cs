using System;
using UnityEngine;

namespace Unidux.Example.Middlewares
{
    [Serializable]
    public class State : StateBase
    {
        public float StartTime;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}