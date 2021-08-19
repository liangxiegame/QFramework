using UnityEngine;

namespace QFramework.Example
{
    public class CTypeTask : TimeTickTask
    {
        public override void OnStart()
        {
            Debug.Log("CTypeTask Start // 这里可以写一些自定义的逻辑");

        }

        public override void OnFinish()
        {
            Debug.Log("CTypeTask Finish // 这里可以写一些自定义的逻辑");
        }
    }
}