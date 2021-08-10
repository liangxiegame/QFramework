using UnityEngine;

namespace QFramework.Example
{
    public class EnumStateMachineExample : MonoBehaviour,
        IOnEvent<EnumStateChangeEvent<EnumStateMachineExample.LeftMouseButtonStates>>
    {
        /// <summary>
        /// 定义枚举状态
        /// </summary>
        public enum LeftMouseButtonStates
        {
            Idle,
            MousePressed
        }
        
        /// <summary>
        /// 定义状态机
        /// </summary>
        private EnumStateMachine<LeftMouseButtonStates> mLeftMouseButtonState;


        private void Start()
        {
            // 第二个参数是，是否发送状态变更事件?
            mLeftMouseButtonState = new EnumStateMachine<LeftMouseButtonStates>(gameObject, true);

            this.RegisterEvent();
        }

        public void OnEvent(EnumStateChangeEvent<LeftMouseButtonStates> e)
        {
            Debug.Log(e.PreviousState + "=>" + e.NewState);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mLeftMouseButtonState.ChangeState(LeftMouseButtonStates.MousePressed);
            }

            if (Input.GetMouseButtonUp(0))
            {
                mLeftMouseButtonState.ChangeState(LeftMouseButtonStates.Idle);
            }
        }

        private void OnDestroy()
        {
            this.UnRegisterEvent();
        }
    }
}