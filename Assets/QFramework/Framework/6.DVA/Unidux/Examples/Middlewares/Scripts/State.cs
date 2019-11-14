using System;
using UnityEngine;

namespace Unidux.Example.Middlewares
{
    [Serializable]
    public class State : DvaState
    {
        public float StartTime;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}