#if UNITY_EDITOR
using NUnit.Framework;

namespace QFramework.Tests
{
    public class FSMTests
    {
        [Test]
        public void FSM_Test()
        {
            //        Idle,               闲置
            //        Run,                跑
            //        Jump,               一段跳
            //        DoubleJump,         二段跳
            //        Die,                挂彩
            var fsm = new QFSMLite();

            // 创建状态
            var idleState = "idle";
            var runState = "run";
            var jumpState = "jump";
            var doubleJumpState = "double_jump";
            var dieState = "die";

            fsm.AddState(idleState);
            fsm.AddState(runState);
            fsm.AddState(jumpState);
            fsm.AddState(doubleJumpState);
            fsm.AddState(dieState);

            // 创建跳转
            var jumpCalled = false;
            var doubleJumpCalled = false;
            var runCalledCount = 0;

            // 添加跳转

            fsm.AddTranslation(runState, "touch_down", jumpState, (_) => { jumpCalled = true; });
            fsm.AddTranslation(jumpState, "touch_down", doubleJumpState, (_) => { doubleJumpCalled = true; });

            fsm.AddTranslation(jumpState, "land", runState, (_) => { runCalledCount++; });

            fsm.AddTranslation(doubleJumpState, "land", runState, (_) => { runCalledCount++; });

            // 初识状态是 runState
            fsm.Start(runState);

            Assert.AreSame(fsm.State, runState);

            // 点击屏幕，进行跳跃
            fsm.HandleEvent("touch_down");

            Assert.IsTrue(jumpCalled);
            Assert.AreSame(fsm.State, jumpState);

            // 点击屏幕，二段跳
            fsm.HandleEvent("touch_down");

            Assert.IsTrue(doubleJumpCalled);
            Assert.AreSame(fsm.State, doubleJumpState);

            // 着陆
            fsm.HandleEvent("land");

            Assert.AreEqual(runCalledCount, 1);
            Assert.AreSame(fsm.State, runState);
        }
    }
}
#endif