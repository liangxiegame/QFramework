#if UNITY_EDITOR
using System;

namespace QFramework
{
    [Serializable]
    public class ResultFormat<T>
    {
        public int code;

        public string msg;

        public T data;
    }
}
#endif