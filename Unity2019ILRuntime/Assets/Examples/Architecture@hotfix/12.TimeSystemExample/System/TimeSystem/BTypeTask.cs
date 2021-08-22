using UnityEngine;

namespace QFramework.Example
{
    public class BTypeTask : TimeTickTask
    {
        public override void OnStart()
        {
            Debug.Log("BTypeTask Start // 这里可以写一些自定义的逻辑");
        }

        public override void OnFinish()
        {
            Debug.Log("BTypeTask Finish // 这里可以写一些自定义的逻辑");
        }
    }
}