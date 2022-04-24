using UnityEngine;

namespace QFramework.Example
{
    /// <summary>
    /// 实验功能谨慎使用
    /// </summary>
    public class UpdateExample : MonoBehaviour
    {
        void Start()
        {
            this.OnUpdate(() => { Debug.Log("Update"); });

            this.OnFixedUpdate(() => { Debug.Log("OnFixedUpdate"); });

            this.OnLateUpdate(() => { Debug.Log("OnLateUpdate"); });

            this.UpdateEvent()
                .Condition(() => Input.GetMouseButtonDown(0))
                .Action(() => { Debug.Log("mouse down"); })
                .Build();
        }
    }
}