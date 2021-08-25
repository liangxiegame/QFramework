using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAndAttribute : ShowIfAttribute
    {
        public ShowIfAndAttribute(params string[] targetNames)
            : base(targetNames)
        {
            mode = Mode.And;
        }
    }
}