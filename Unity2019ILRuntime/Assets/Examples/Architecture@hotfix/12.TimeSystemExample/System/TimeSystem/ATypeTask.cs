using UnityEngine;

namespace QFramework.Example
{
    public class ATypeTask : TimeTickTask
    {
        public override void OnStart()
        {
            Debug.Log("ATypeTask Start // 这里可以写一些自定义的逻辑");
        }

        public override void OnFinish()
        {
            Debug.Log("ATypeTask Finish // 这里可以写一些自定义的逻辑");
        }
    }
}