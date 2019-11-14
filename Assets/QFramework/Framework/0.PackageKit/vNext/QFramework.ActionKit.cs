using System;
using System.Collections.Generic;
using System.Linq;
using QF;
using UniRx;
#if UNITY_5_6_OR_NEWER
using System.Collections;
using UnityEngine;

#endif

// ActionKit 以後往可視化編程的方向發展
namespace QFramework
{
    /// <summary>
    /// 执行节点的基类
    /// </summary>
    public interface IAction : IDisposable
    {
        bool Disposed { get; }

        bool Execute(float delta);

        void Reset();

        void Finish();

        bool Finished { get; }
    }


    public abstract class NodeAction : IAction
    {
        public Action OnBeganCallback    = null;
        public Action OnEndedCallback    = null;
        public Action OnDisposedCallback = null;

        protected bool mOnBeginCalled = false;

        #region IAction Support

        bool IAction.Disposed
        {
            get { return mDisposed; }
        }

        protected bool mDisposed = false;

        public bool Finished { get; protected set; }

        public virtual void Finish()
        {
            Finished = true;
            if (OnEndedCallback != null)
            {
                OnEndedCallback.Invoke();
            }

            OnEnd();
        }

        public void Break()
        {
            Finished = true;
        }

        #endregion

        #region ResetableSupport

        public void Reset()
        {
            Finished = false;
            mOnBeginCalled = false;
            mDisposed = false;
            OnReset();
        }

        #endregion


        #region IExecutable Support

        public bool Execute(float dt)
        {
            // 有可能被别的地方调用
            if (Finished)
            {
                return Finished;
            }

            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();

                if (OnBeganCallback != null)
                {
                    OnBeganCallback.Invoke();
                }
            }

            if (!Finished)
            {
                OnExecute(dt);
            }

            if (Finished)
            {
                Finish();
            }

            return Finished || mDisposed;
        }

        #endregion

        protected virtual void OnReset()
        {
        }

        protected virtual void OnBegin()
        {
        }

        /// <summary>
        /// finished
        /// </summary>
        protected virtual void OnExecute(float dt)
        {
        }

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnDispose()
        {
        }

        #region IDisposable Support

        public void Dispose()
        {
            if (mDisposed) return;
            mDisposed = true;

            OnBeganCallback = null;
            OnEndedCallback = null;

            if (OnDisposedCallback != null)
            {
                OnDisposedCallback.Invoke();
            }

            OnDisposedCallback = null;
            OnDispose();
        }

        #endregion
    }

    public interface IDisposeWhen : IDisposeEventRegister
    {
        IDisposeEventRegister DisposeWhen(Func<bool> condition);
    }

    public interface IDisposeEventRegister
    {
        void OnDisposed(System.Action onDisposedEvent);

        IDisposeEventRegister OnFinished(Action onFinishedEvent);
    }

#if UNITY_5_6_OR_NEWER
    public static class IActionExtension
    {
        public static T ExecuteNode<T>(this T selBehaviour, IAction commandNode) where T : MonoBehaviour
        {
            selBehaviour.StartCoroutine(commandNode.Execute());
            return selBehaviour;
        }

        public static void Delay<T>(this T selfBehaviour, float seconds, System.Action delayEvent)
            where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(IActionChainExtention.DelayAction.Allocate(seconds, delayEvent));
        }

        public static IEnumerator Execute(this IAction selfNode)
        {
            if (selfNode.Finished) selfNode.Reset();

            while (!selfNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        }
    }

    public abstract class ActionChain : NodeAction, IActionChain, IDisposeWhen
    {
        public MonoBehaviour Executer { get; set; }

        protected abstract NodeAction mNode { get; }

        public abstract IActionChain Append(IAction node);

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition != null && mDisposeCondition.Invoke())
            {
                Finish();
            }
            else
            {
                Finished = mNode.Execute(dt);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            Dispose();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Executer = null;
            mDisposeWhenCondition = false;
            mDisposeCondition = null;

            if (mOnDisposedEvent != null)
            {
                mOnDisposedEvent.Invoke();
            }

            mOnDisposedEvent = null;
        }

        public IDisposeWhen Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }

        private bool       mDisposeWhenCondition = false;
        private Func<bool> mDisposeCondition;
        private Action     mOnDisposedEvent = null;

        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }

        IDisposeEventRegister IDisposeEventRegister.OnFinished(Action onFinishedEvent)
        {
            OnEndedCallback += onFinishedEvent;
            return this;
        }

        public void OnDisposed(System.Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }
    }

    /// <summary>
    /// 执行链表,
    /// </summary>
    public interface IActionChain : IAction
    {
        MonoBehaviour Executer { get; set; }

        IActionChain Append(IAction node);

        IDisposeWhen Begin();
    }

    public static partial class IActionChainExtention
    {
        public static IActionChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count) {Executer = selfbehaviour};
            retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain {Executer = selfbehaviour};
            retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain OnlyBegin(this IActionChain selfChain, Action<OnlyBeginAction> onBegin)
        {
            return selfChain.Append(OnlyBeginAction.Allocate(onBegin));
        }

        public static IActionChain Delay(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        /// <summary>
        /// Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IActionChain Wait(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        public static IActionChain Event(this IActionChain selfChain, params System.Action[] onEvents)
        {
            return selfChain.Append(EventAction.Allocate(onEvents));
        }


        public static IActionChain Until(this IActionChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilAction.Allocate(condition));
        }

        /// <inheritdoc />
        /// <summary>
        /// 延时执行节点
        /// </summary>
        public class DelayAction : NodeAction, IPoolable
        {
            public float DelayTime;

            public static DelayAction Allocate(float delayTime, System.Action onEndCallback = null)
            {
                var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
                retNode.DelayTime = delayTime;
                retNode.OnEndedCallback = onEndCallback;
                return retNode;
            }

            public DelayAction()
            {
            }

            public DelayAction(float delayTime)
            {
                DelayTime = delayTime;
            }

            private float mCurrentSeconds = 0.0f;

            protected override void OnReset()
            {
                mCurrentSeconds = 0.0f;
            }

            protected override void OnExecute(float dt)
            {
                mCurrentSeconds += dt;
                Finished = mCurrentSeconds >= DelayTime;
            }

            protected override void OnDispose()
            {
                SafeObjectPool<DelayAction>.Instance.Recycle(this);
            }

            public void OnRecycled()
            {
                DelayTime = 0.0f;
                Reset();
            }

            public bool IsRecycled { get; set; }

        }

        /// <inheritdoc />
        /// <summary>
        /// 延时执行节点
        /// </summary>
        public class EventAction : NodeAction, IPoolable
        {
            private System.Action mOnExecuteEvent;

            /// <summary>
            /// TODO:这里填可变参数会有问题
            /// </summary>
            /// <param name="onExecuteEvents"></param>
            /// <returns></returns>
            public static EventAction Allocate(params System.Action[] onExecuteEvents)
            {
                var retNode = SafeObjectPool<EventAction>.Instance.Allocate();
                Array.ForEach(onExecuteEvents, onExecuteEvent => retNode.mOnExecuteEvent += onExecuteEvent);
                return retNode;
            }

            /// <summary>
            /// finished
            /// </summary>
            protected override void OnExecute(float dt)
            {
                if (mOnExecuteEvent != null)
                {
                    mOnExecuteEvent.Invoke();
                }

                Finished = true;
            }

            protected override void OnDispose()
            {
                SafeObjectPool<EventAction>.Instance.Recycle(this);
            }

            public void OnRecycled()
            {
                Reset();
                mOnExecuteEvent = null;
            }

            public bool IsRecycled { get; set; }

        }

        public class NodeActionSystem
        {
            [RuntimeInitializeOnLoadMethod]
            private static void InitNodeSystem()
            {
                // cache list			

                // cache node
                SafeObjectPool<DelayAction>.Instance.Init(50, 50);
                SafeObjectPool<EventAction>.Instance.Init(50, 50);
            }
        }

        public class KeyEventAction : EventAction
        {
            private TimelineNode mTimelineNode;
            private string       mKeyEventName;

            public KeyEventAction(string keyEventName, TimelineNode timelineNode)
            {
                mTimelineNode = timelineNode;
                mKeyEventName = keyEventName;
            }

            protected override void OnExecute(float dt)
            {
                mTimelineNode.OnKeyEventsReceivedCallback(mKeyEventName);
                Finished = true;
            }

            protected override void OnDispose()
            {
                mTimelineNode = null;
                mKeyEventName = null;
            }
        }
    }

    public class RepeatNodeChain : ActionChain
    {
        protected override NodeAction mNode
        {
            get { return mRepeatAction; }
        }

        private RepeatNode mRepeatAction;

        private SequenceNode mSequenceNode;

        public RepeatNodeChain(int repeatCount)
        {
            mSequenceNode = new SequenceNode();
            mRepeatAction = new RepeatNode(mSequenceNode, repeatCount);
        }

        public override IActionChain Append(IAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mRepeatAction)
            {
                mRepeatAction.Dispose();
            }

            mRepeatAction = null;

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }

    /// <summary>
    /// 支持链式方法
    /// </summary>
    public class SequenceNodeChain : ActionChain
    {
        protected override NodeAction mNode
        {
            get { return mSequenceNode; }
        }

        private SequenceNode mSequenceNode;

        public SequenceNodeChain()
        {
            mSequenceNode = new SequenceNode();
        }

        public override IActionChain Append(IAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }

    public class OnlyBeginAction : NodeAction, IPoolable, IPoolType
    {
        private Action<OnlyBeginAction> mBeginAction;

        public static OnlyBeginAction Allocate(Action<OnlyBeginAction> beginAction)
        {
            var retSimpleAction = SafeObjectPool<OnlyBeginAction>.Instance.Allocate();

            retSimpleAction.mBeginAction = beginAction;

            return retSimpleAction;
        }

        public void OnRecycled()
        {
            mBeginAction = null;
        }

        protected override void OnBegin()
        {
            if (mBeginAction != null)
            {
                mBeginAction.Invoke(this);
            }
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<OnlyBeginAction>.Instance.Recycle(this);
        }
    }


    /// <inheritdoc />
    /// <summary>
    /// like filter, add condition
    /// </summary>
    public class UntilAction : NodeAction, IPoolable
    {
        private Func<bool> mCondition;

        public static UntilAction Allocate(Func<bool> condition)
        {
            var retNode = SafeObjectPool<UntilAction>.Instance.Allocate();
            retNode.mCondition = condition;
            return retNode;
        }

        protected override void OnExecute(float dt)
        {
            Finished = mCondition.Invoke();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<UntilAction>.Instance.Recycle(this);
        }

        void IPoolable.OnRecycled()
        {
            Reset();
            mCondition = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }

    public interface INode
    {
        IAction CurrentExecutingNode { get; }
    }

    public class RepeatNode : NodeAction, INode
    {
        public RepeatNode(IAction node, int repeatCount)
        {
            RepeatCount = repeatCount;
            mNode = node;
        }

        public IAction CurrentExecutingNode
        {
            get
            {
                var currentNode = mNode;
                var node = currentNode as INode;
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }

        private IAction mNode;

        public int RepeatCount = 1;

        private int mCurRepeatCount = 0;

        protected override void OnReset()
        {
            if (null != mNode)
            {
                mNode.Reset();
            }

            mCurRepeatCount = 0;
            Finished = false;
        }

        protected override void OnExecute(float dt)
        {
            if (RepeatCount == -1)
            {
                if (mNode.Execute(dt))
                {
                    mNode.Reset();
                }

                return;
            }

            if (mNode.Execute(dt))
            {
                mNode.Reset();
                mCurRepeatCount++;
            }

            if (mCurRepeatCount == RepeatCount)
            {
                Finished = true;
            }
        }

        protected override void OnDispose()
        {
            if (null != mNode)
            {
                mNode.Dispose();
                mNode = null;
            }
        }
    }


    /// <summary>
    /// 序列执行节点
    /// </summary>
    public class SequenceNode : NodeAction, INode
    {
        protected readonly List<IAction> mNodes         = new List<IAction>();
        protected readonly List<IAction> mExcutingNodes = new List<IAction>();

        public int TotalCount
        {
            get { return mExcutingNodes.Count; }
        }

        public IAction CurrentExecutingNode
        {
            get
            {
                var currentNode = mExcutingNodes[0];
                var node = currentNode as INode;
                Log.E(node);
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }

        protected override void OnReset()
        {
            mExcutingNodes.Clear();
            foreach (var node in mNodes)
            {
                node.Reset();
                mExcutingNodes.Add(node);
            }
        }

        protected override void OnExecute(float dt)
        {
            if (mExcutingNodes.Count > 0)
            {
                // 如果有异常，则进行销毁，不再进行下边的操作
                if (mExcutingNodes[0].Disposed && !mExcutingNodes[0].Finished)
                {
                    Dispose();
                    return;
                }

                while (mExcutingNodes[0].Execute(dt))
                {
                    mExcutingNodes.RemoveAt(0);

                    OnCurrentActionFinished();

                    if (mExcutingNodes.Count == 0)
                    {
                        break;
                    }
                }
            }

            Finished = mExcutingNodes.Count == 0;
        }

        protected virtual void OnCurrentActionFinished()
        {
        }

        public SequenceNode(params IAction[] nodes)
        {
            foreach (var node in nodes)
            {
                mNodes.Add(node);
                mExcutingNodes.Add(node);
            }
        }

        public SequenceNode Append(IAction appendedNode)
        {
            mNodes.Add(appendedNode);
            mExcutingNodes.Add(appendedNode);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mNodes)
            {
                mNodes.ForEach(node => node.Dispose());
                mNodes.Clear();
            }

            if (null != mExcutingNodes)
            {
                mExcutingNodes.Clear();
            }
        }
    }


    /// <summary>
    /// 并发执行的协程
    /// </summary>
    public class SpawnNode : NodeAction
    {
        protected List<NodeAction> mNodes = new List<NodeAction>();

        protected override void OnReset()
        {
            mNodes.ForEach(node => node.Reset());
            mFinishCount = 0;
        }

        public override void Finish()
        {
            for (var i = mNodes.Count - 1; i >= 0; i--)
            {
                mNodes[i].Finish();
            }

            base.Finish();
        }

        protected override void OnExecute(float dt)
        {
            for (var i = mNodes.Count - 1; i >= 0; i--)
            {
                var node = mNodes[i];
                if (!node.Finished && node.Execute(dt))
                    Finished = mNodes.Count == mFinishCount;
            }
        }

        private int mFinishCount = 0;

        private void IncreaseFinishCount()
        {
            mFinishCount++;
        }

        public SpawnNode(params NodeAction[] nodes)
        {
            mNodes.AddRange(nodes);

            foreach (var nodeAction in nodes)
            {
                nodeAction.OnEndedCallback += IncreaseFinishCount;
            }
        }

        public void Add(params NodeAction[] nodes)
        {
            mNodes.AddRange(nodes);

            foreach (var nodeAction in nodes)
            {
                nodeAction.OnEndedCallback += IncreaseFinishCount;
            }
        }

        protected override void OnDispose()
        {
            foreach (var node in mNodes)
            {
                node.OnEndedCallback -= IncreaseFinishCount;
                node.Dispose();
            }

            mNodes.Clear();
            mNodes = null;
        }
    }

    /// <summary>
    /// 时间轴执行节点
    /// </summary>
    public class TimelineNode : NodeAction
    {
        private float mCurTime = 0;

        public System.Action OnTimelineBeganCallback
        {
            get { return OnBeganCallback; }
            set { OnBeganCallback = value; }
        }

        public System.Action OnTimelineEndedCallback
        {
            get { return OnEndedCallback; }
            set { OnEndedCallback = value; }
        }

        public Action<string> OnKeyEventsReceivedCallback = null;

        public class TimelinePair
        {
            public float   Time;
            public IAction Node;

            public TimelinePair(float time, IAction node)
            {
                Time = time;
                Node = node;
            }
        }

        /// <summary>
        /// refator 2 one list? all in one list;
        /// </summary>
        public Queue<TimelinePair> TimelineQueue = new Queue<TimelinePair>();

        protected override void OnReset()
        {
            mCurTime = 0.0f;

            foreach (var timelinePair in TimelineQueue)
            {
                timelinePair.Node.Reset();
            }
        }

        protected override void OnExecute(float dt)
        {
            mCurTime += dt;

            foreach (var pair in TimelineQueue.Where(pair => pair.Time < mCurTime && !pair.Node.Finished))
            {
                if (pair.Node.Execute(dt))
                {
                    Finished = TimelineQueue.Where(timetinePair => !timetinePair.Node.Finished).Count() == 0;
                }
            }
        }

        public TimelineNode(params TimelinePair[] pairs)
        {
            foreach (var pair in pairs)
            {
                TimelineQueue.Enqueue(pair);
            }
        }

        public void Append(TimelinePair pair)
        {
            TimelineQueue.Enqueue(pair);
        }

        public void Append(float time, IAction node)
        {
            TimelineQueue.Enqueue(new TimelinePair(time, node));
        }

        protected override void OnDispose()
        {
            foreach (var timelinePair in TimelineQueue)
            {
                timelinePair.Node.Dispose();
            }

            TimelineQueue.Clear();
            TimelineQueue = null;
        }
    }

    /// <summary>
    /// 启动执行节点
    /// </summary>
    public class ProcessNode : NodeAction
    {
        protected string mTips = "Default";

        public virtual float Progress { get; set; }

        public virtual string Tips
        {
            get { return mTips; }
            set { mTips = value; }
        }
    }

    /// <summary>
    /// 事件注入,和 NodeSystem 配套使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventInjector<T>
    {
        public delegate bool InjectEventTrigger(T lastValue, T newValue);

        public delegate T InjectEventGetter();

        private T mCahedLastValue;

        public readonly Func<T> mGetter;

        public T Value
        {
            get { return mCahedLastValue; }
        }

        public EventInjector(Func<T> getter)
        {
            mGetter = getter;
        }

        public bool GetOn(InjectEventTrigger triggerConditionWithOldAndNewValue)
        {
            var value = mGetter();
            var trig = triggerConditionWithOldAndNewValue(mCahedLastValue, value);
            mCahedLastValue = value;
            return trig;
        }

        public bool GetOnValueChanged(Func<T, bool> triggerConditionWithNewValue = null)
        {
            return GetOn((lastValue, newValue) =>
                lastValue.Equals(newValue) &&
                (triggerConditionWithNewValue == null || triggerConditionWithNewValue(newValue)));
        }
    }

    [MonoSingletonPath("[ActionKit]/ActionQueue")]
    public class ActionQueue : MonoBehaviour, ISingleton
    {
        private List<IAction> mActions = new List<IAction>();

        public static void Append(IAction action)
        {
            mInstance.mActions.Add(action);
        }

        // Update is called once per frame
        private void Update()
        {
            if (mActions.Count != 0 && mActions[0].Execute(Time.deltaTime))
            {
                mActions.RemoveAt(0);
            }
        }

        void ISingleton.OnSingletonInit()
        {
        }

        private static ActionQueue mInstance
        {
            get { return MonoSingletonProperty<ActionQueue>.Instance; }
        }
    }
#endif

#if UNITY_EDITOR
    public static class EditorActionKit
    {
        public static void ExecuteNode(NodeAction nodeAction)
        {
            new NodeActionEditorWrapper(nodeAction);
        }
    }

    public class NodeActionEditorWrapper
    {
        private NodeAction mNodeAction;

        public NodeActionEditorWrapper(NodeAction action)
        {
            mNodeAction = action;
            UnityEditor.EditorApplication.update += Update;
            mNodeAction.OnEndedCallback += () => { UnityEditor.EditorApplication.update -= Update; };
        }

        void Update()
        {
            if (!mNodeAction.Finished && mNodeAction.Execute(Time.deltaTime))
            {
                mNodeAction.Dispose();
                mNodeAction = null;
            }
        }
    }
#endif
}