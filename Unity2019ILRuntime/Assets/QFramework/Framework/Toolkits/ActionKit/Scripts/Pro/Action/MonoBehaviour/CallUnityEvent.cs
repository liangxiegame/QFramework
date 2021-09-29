using UnityEngine.Events;

namespace QFramework
{
    /// <summary>
    /// 调用方法
    /// </summary>
    [ActionGroup("MonoBehaviour")]
    public class CallUnityEvent : ActionKitVisualAction
    {
        public UnityEvent Event = new UnityEvent();

        protected override void OnBegin()
        {
            base.OnBegin();

            if (Event != null)
            {
                Event.Invoke();
            }

            Finish();
        }
    }
}