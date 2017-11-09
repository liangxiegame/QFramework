using UnityEngine;

namespace QFramework.VisualDebugging.Unity
{
    public class DebugSystemsBehaviour : MonoBehaviour
    {
        public DebugSystems Systems { get; protected set; }


        public void Init(DebugSystems systems)
        {
            Systems = systems;
        }
    }
}