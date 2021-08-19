using UnityEngine;

namespace QFramework.Example
{
    public class MouseUpDecision : AIDecision
    {
        public override bool Decide()
        {
            return Input.GetMouseButtonUp(0);
        }
    }
}