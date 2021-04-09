using UnityEngine;

namespace QFramework.Example
{
    public class MouseDownDecision : AIDecision
    {
        public override bool Decide()
        {
            return Input.GetMouseButtonDown(0);
        }
    }
}