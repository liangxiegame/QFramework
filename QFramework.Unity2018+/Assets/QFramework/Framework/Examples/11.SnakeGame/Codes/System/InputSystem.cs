using UnityEngine;
using QFramework;

namespace SnakeGame
{
    public interface IInputSystem : ISystem { }
    public class InputSystem : AbstractSystem, IInputSystem
    {
        private DirInputEvent mDirInput = new DirInputEvent();
        /// <summary>
        /// 用于表示输入状态
        /// </summary>
        private enum E_InputState { Down, Up, Hold, Null }
        //按键集成
        private readonly KeyCode[] mUpKeys = { KeyCode.W, KeyCode.UpArrow };
        private readonly KeyCode[] mDownKeys = { KeyCode.S, KeyCode.DownArrow };
        private readonly KeyCode[] mLeftKeys = { KeyCode.A, KeyCode.LeftArrow };
        private readonly KeyCode[] mRightKeys = { KeyCode.D, KeyCode.RightArrow };
        /// <summary>
        /// 更新函数
        /// </summary>
        private void OnUpdate()
        {
            CheckMoveDir(BindableInput(mLeftKeys), BindableInput(mRightKeys), ref mDirInput.hor);
            CheckMoveDir(BindableInput(mDownKeys), BindableInput(mUpKeys), ref mDirInput.ver);
            this.SendEvent(mDirInput);
        }
        /// <summary>
        /// 检测移动方向
        /// </summary>
        /// <param name="negativeState">负向量状态 => [-1]</param>
        /// <param name="positiveState">正向量状态 => [+1]</param>
        /// <param name="axisName">输入轴名称</param>
        /// <param name="moveDir">返回的向量值</param>
        private void CheckMoveDir(E_InputState negativeState, E_InputState positiveState, ref int moveDir)
        {
            //如果按下负方向 或 轴心为负数 return -1
            if (negativeState == E_InputState.Down) moveDir = -1;
            //如果按下正方向 或 轴心为正数 return 1
            else if (positiveState == E_InputState.Down) moveDir = 1;
            //如果抬起正方向,如果还按着负方向 return -1 否则 return 0
            else if (positiveState == E_InputState.Up)
                moveDir = negativeState == E_InputState.Hold ? -1 : 0;
            //如果抬起负方向,如果还按着正方向 return 1 否则 return 0
            else if (negativeState == E_InputState.Up)
                moveDir = positiveState == E_InputState.Hold ? 1 : 0;
        }
        /// <summary>
        /// 绑定按键输入【多情况】
        /// </summary>
        /// <param name="keys">按键数组</param>
        private E_InputState BindableInput(KeyCode[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i])) return E_InputState.Down;
                if (Input.GetKeyUp(keys[i])) return E_InputState.Up;
                if (Input.GetKey(keys[i])) return E_InputState.Hold;
            }
            return E_InputState.Null;
        }
        protected override void OnInit() => CommonMono.AddUpdateAction(OnUpdate);
    }
}