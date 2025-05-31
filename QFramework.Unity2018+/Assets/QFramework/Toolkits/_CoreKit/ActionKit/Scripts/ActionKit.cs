/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using System.Threading.Tasks;

namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.164
    [ClassAPI("04.ActionKit", "ActionKit", 0, "ActionKit")]
    [APIDescriptionCN("Action 时序动作序列（组合模式 + 命令模式 + 建造者模式）")]
    [APIDescriptionEN("Action Sequence (composite pattern + command pattern + builder pattern)")]
#endif
    public partial class ActionKit : Architecture<ActionKit>
    {
        public static ulong ID_GENERATOR = 0;
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("延时回调")]
        [APIDescriptionEN("delay callback")]
        [APIExampleCode(@"
Debug.Log(""Start Time:"" + Time.time);
 
ActionKit.Delay(1.0f, () =>
{
    Debug.Log(""End Time:"" + Time.time);
             
}).Start(this); // update driven
 
// Start Time: 0.000000
---- after 1 seconds ----
---- 一秒后 ----
// End Time: 1.000728
")]
#endif
        public static IAction Delay(float seconds, Action callback)
        {
            return QFramework.Delay.Allocate(seconds, callback);
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("动作序列")]
        [APIDescriptionEN("action sequence")]
        [APIExampleCode(@"
Debug.Log(""Sequence Start:"" + Time.time);
 
ActionKit.Sequence()
    .Callback(() => Debug.Log(""Delay Start:"" + Time.time))
    .Delay(1.0f)
    .Callback(() => Debug.Log(""Delay Finish:"" + Time.time))
    .Start(this, _ => { Debug.Log(""Sequence Finish:"" + Time.time); });
 
// Sequence Start: 0
// Delay Start: 0
------ after 1 seconds ------
------ 1 秒后 ------
// Delay Finish: 1.01012
// Sequence Finish: 1.01012
")]
#endif
        public static ISequence Sequence()
        {
            return QFramework.Sequence.Allocate();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("延时帧")]
        [APIDescriptionEN("delay by frameCount")]
        [APIExampleCode(@"
Debug.Log(""Delay Frame Start FrameCount:"" + Time.frameCount);
 
ActionKit.DelayFrame(1, () => { Debug.Log(""Delay Frame Finish FrameCount:"" + Time.frameCount); })
        .Start(this);
 
ActionKit.Sequence()
        .DelayFrame(10)
        .Callback(() => Debug.Log(""Sequence Delay FrameCount:"" + Time.frameCount))
        .Start(this);

// Delay Frame Start FrameCount:1
// Delay Frame Finish FrameCount:2
// Sequence Delay FrameCount:11
 
// --- also support nextFrame
// --- 还可以用 NextFrame  
// ActionKit.Sequence()
//      .NextFrame()
//      .Start(this);
//
// ActionKit.NextFrame(() => { }).Start(this);
")]
#endif
        public static IAction DelayFrame(int frameCount, Action onDelayFinish)
        {
            return QFramework.DelayFrame.Allocate(frameCount, onDelayFinish);
        }

        public static IAction NextFrame(Action onNextFrame)
        {
            return QFramework.DelayFrame.Allocate(1, onNextFrame);
        }


        public static IAction Lerp(float a,float b,float duration,Action<float> onLerp,Action onLerpFinish = null)
        {
            return QFramework.Lerp.Allocate(a, b, duration, onLerp, onLerpFinish);
        }

        public static IAction Callback(Action callback)
        {
            return QFramework.CallbackAction.Allocate(callback);
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("条件")]
        [APIDescriptionEN("condition action")]
        [APIExampleCode(@"
ActionKit.Sequence()
        .Callback(() => Debug.Log(""Before Condition""))
        .Condition(() => Input.GetMouseButtonDown(0))
        .Callback(() => Debug.Log(""Mouse Clicked""))
        .Start(this);

ActionKit.Condition(()=>Input.GetKeyDown(KeyCode.Space),()=>Debug.Log(\""Space Down\""))
    .Start(this);

// Before Condition
// ---- after left mouse click ----
// ---- 鼠标左键点击之后 ----
// Mouse Clicked
// ---- 空格按下之后 ----
// Space Down
")]
#endif

        public static IAction Condition(Func<bool> condition, Action onCondition = null)
        {
            return ConditionAction.Allocate(condition, onCondition);
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("重复动作")]
        [APIDescriptionEN("repeat action")]
        [APIExampleCode(@"
ActionKit.Repeat()
        .Condition(() => Input.GetMouseButtonDown(0))
        .Callback(() => Debug.Log(""Mouse Clicked""))
        .Start(this);
// always Log Mouse Clicked when click left mouse
// 鼠标左键点击时，每次都会输出 Mouse Clicked

ActionKit.Repeat(5) // -1、0 means forever 1 means once  2 means twice
        .Condition(() => Input.GetMouseButtonDown(1))
        .Callback(() => Debug.Log(""Mouse right clicked""))
        .Start(this, () =>
        {
            Debug.Log(""Right click finished"");
        });
// Mouse right clicked
// Mouse right clicked
// Mouse right clicked
// Mouse right clicked
// Mouse right clicked
// Right click finished
    ")]
#endif
        public static IRepeat Repeat(int repeatCount = -1)
        {
            return QFramework.Repeat.Allocate(repeatCount);
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("并行动作")]
        [APIDescriptionEN("parallel action")]
        [APIExampleCode(@"
Debug.Log(""Parallel Start:"" + Time.time);
 
ActionKit.Parallel()
        .Delay(1.0f, () => { Debug.Log(Time.time); })
        .Delay(2.0f, () => { Debug.Log(Time.time); })
        .Delay(3.0f, () => { Debug.Log(Time.time); })
        .Start(this, () =>
        {
            Debug.Log(""Parallel Finish:"" + Time.time);
        });
// Parallel Start:0
// 1.01
// 2.01
// 3.02
// Parallel Finish:3.02
")]
#endif
        public static IParallel Parallel()
        {
            return QFramework.Parallel.Allocate();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("复合动作示例")]
        [APIDescriptionEN("Complex action example")]
        [APIExampleCode(@"
ActionKit.Sequence()
        .Callback(() => Debug.Log(""Sequence Start""))
        .Callback(() => Debug.Log(""Parallel Start""))
        .Parallel(p =>
        {
            p.Delay(1.0f, () => Debug.Log(""Delay 1s Finished""))
                .Delay(2.0f, () => Debug.Log(""Delay 2s Finished""));
        })
        .Callback(() => Debug.Log(""Parallel Finished""))
        .Callback(() => Debug.Log(""Check Mouse Clicked""))
        .Sequence(s =>
        {
            s.Condition(() => Input.GetMouseButton(0))
                .Callback(() => Debug.Log(""Mouse Clicked""));
        })
        .Start(this, () =>
        {
            Debug.Log(""Finish"");
        });
// 
// Sequence Start
// Parallel Start
// Delay 1s Finished
// Delay 2s Finished
// Parallel Finished
// Check Mouse Clicked
// ------ After Left Mouse Clicked ------
// ------ 鼠标左键点击后 ------
// Mouse Clicked
// Finish

")]
#endif
        public void ComplexAPI()
        {
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("自定义动作")]
        [APIDescriptionEN("Custom action example")]
        [APIExampleCode(@" 
ActionKit.Custom(a =>
{
    a
        .OnStart(() => { Debug.Log(""OnStart""); })
        .OnExecute(dt =>
        {
            Debug.Log(""OnExecute"");
 
            a.Finish();
        })
        .OnFinish(() => { Debug.Log(""OnFinish""); });
}).Start(this);
             
// OnStart
// OnExecute
// OnFinish
 
class SomeData
{
    public int ExecuteCount = 0;
}
 
ActionKit.Custom<SomeData>(a =>
{
    a
        .OnStart(() =>
        {
            a.Data = new SomeData()
            {
                ExecuteCount = 0
            };
        })
        .OnExecute(dt =>
        {
            Debug.Log(a.Data.ExecuteCount);
            a.Data.ExecuteCount++;
 
            if (a.Data.ExecuteCount >= 5)
            {
                a.Finish();
            }
        }).OnFinish(() => { Debug.Log(""Finished""); });
}).Start(this);
         
// 0
// 1
// 2
// 3
// 4
// Finished
 
// 还支持 Sequence、Repeat、Parallel 等
// Also support sequence repeat Parallel
// ActionKit.Sequence()
//     .Custom(c =>
//     {
//         c.OnStart(() => c.Finish());
//     }).Start(this);
")]
#endif
        public static IAction Custom(Action<ICustomAPI<object>> customSetting)
        {
            var action = QFramework.Custom.Allocate();
            customSetting(action);
            return action;
        }

        public static IAction Custom<TData>(Action<ICustomAPI<TData>> customSetting)
        {
            var action = QFramework.Custom<TData>.Allocate();
            customSetting(action);
            return action;
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("协程支持")]
        [APIDescriptionEN("coroutine action example")]
        [APIExampleCode(@"
IEnumerator SomeCoroutine()
{
    yield return new WaitForSeconds(1.0f);
    Debug.Log(""Hello:"" + Time.time);
}
 
ActionKit.Coroutine(SomeCoroutine).Start(this);
// Hello:1.0039           
SomeCoroutine().ToAction().Start(this);
// Hello:1.0039
ActionKit.Sequence()
    .Coroutine(SomeCoroutine)
    .Start(this);
// Hello:1.0039
")]
#endif
        public static IAction Coroutine(Func<IEnumerator> coroutineGetter)
        {
            return CoroutineAction.Allocate(coroutineGetter);
        }
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("Task 支持")]
        [APIDescriptionEN("Task action example")]
        [APIExampleCode(@"
async Task SomeTask()
{
    await Task.Delay(TimeSpan.FromSeconds(1.0f));
    Debug.Log(""Hello:"" + Time.time);
}

ActionKit.Task(SomeTask).Start(this);

SomeTask().ToAction().Start(this);

ActionKit.Sequence()
    .Task(SomeTask)
    .Start(this);

// Hello:1.0039
")]
#endif
        public static IAction Task(Func<Task> taskGetter)
        {
            return TaskAction.Allocate(taskGetter);
        }



        #region Events
        
#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("Update 生命周期支持")]
        [APIDescriptionEN("mono update life-circle event support")]
        [APIExampleCode(@"
ActionKit.OnUpdate.Register(() =>
{
    if (Time.frameCount % 30 == 0)
    {
        Debug.Log(""Update"");
    }
}).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif

        public static EasyEvent OnUpdate => ActionKitMonoBehaviourEvents.Instance.OnUpdate;
        
#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("FixedUpdate 生命周期支持")]
        [APIDescriptionEN("mono fixed update life-circle event support")]
        [APIExampleCode(@"
ActionKit.OnFixedUpdate.Register(() =>
{
    // fixed update code here
    // 这里写 fixed update 相关代码
}).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif
        public static EasyEvent OnFixedUpdate => ActionKitMonoBehaviourEvents.Instance.OnFixedUpdate;
        
#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("LateUpdate 生命周期支持")]
        [APIDescriptionEN("mono late update life-circle event support")]
        [APIExampleCode(@"
ActionKit.OnLateUpdate.Register(() =>
{
    // late update code here
    // 这里写 late update 相关代码
}).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif
        public static EasyEvent OnLateUpdate => ActionKitMonoBehaviourEvents.Instance.OnLateUpdate;
        
#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("OnGUI 生命周期支持")]
        [APIDescriptionEN("mono on gui life-circle event support")]
        [APIExampleCode(@"
ActionKit.OnGUI.Register(() =>
{
    GUILayout.Label(""See Example Code"");
    GUILayout.Label(""请查看示例代码"");
}).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif
        public static EasyEvent OnGUI => ActionKitMonoBehaviourEvents.Instance.OnGUIEvent;
        
#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("OnApplicationQuit 生命周期支持")]
        [APIDescriptionEN("mono OnApplicationQuit life-circle event support")]
        [APIExampleCode(@"
ActionKit.OnApplicationQuit.Register(() =>
{
    Debug.Log(""quit"");
}).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif
        public static EasyEvent OnApplicationQuit => ActionKitMonoBehaviourEvents.Instance.OnApplicationQuitEvent;

#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("OnApplicationPause 生命周期支持")]
        [APIDescriptionEN("mono OnApplicationPause life-circle event support")]
        [APIExampleCode(@"
            ActionKit.OnApplicationPause.Register(pause =>
            {
                Debug.Log(""pause:"" + pause);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif
        public static EasyEvent<bool> OnApplicationPause =>
            ActionKitMonoBehaviourEvents.Instance.OnApplicationPauseEvent;

        
#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("OnApplicationFocus 生命周期支持")]
        [APIDescriptionEN("mono OnApplicationFocus life-circle event support")]
        [APIExampleCode(@"
ActionKit.OnApplicationFocus.Register(focus =>
{
    Debug.Log(""focus:"" + focus);
}).UnRegisterWhenGameObjectDestroyed(gameObject);
")]
#endif
        public static EasyEvent<bool> OnApplicationFocus =>
            ActionKitMonoBehaviourEvents.Instance.OnApplicationFocusEvent;

        protected override void Init()
        {
        }

        #endregion
    }
}