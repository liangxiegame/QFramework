/****************************************************************************
 * Copyright (c) 2016 - 2025 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IState
    {
        bool Condition();
        void Enter();
        void Update();
        void FixedUpdate();
        void OnGUI();
        void Exit();
    }
    
    
    public class CustomState : IState
    {
        private Func<bool> mOnCondition;
        private Action mOnEnter;
        private Action mOnUpdate;
        private Action mOnFixedUpdate;
        private Action mOnGUI;
        private Action mOnExit;

        public CustomState OnCondition(Func<bool> onCondition)
        {
            mOnCondition = onCondition;
            return this;
        }
        
        public CustomState OnEnter(Action onEnter)
        {
            mOnEnter = onEnter;
            return this;
        }

        
        public CustomState OnUpdate(Action onUpdate)
        {
            mOnUpdate = onUpdate;
            return this;
        }
        
        public CustomState OnFixedUpdate(Action onFixedUpdate)
        {
            mOnFixedUpdate = onFixedUpdate;
            return this;
        }
        
        public CustomState OnGUI(Action onGUI)
        {
            mOnGUI = onGUI;
            return this;
        }
        
        public CustomState OnExit(Action onExit)
        {
            mOnExit = onExit;
            return this;
        }


        public bool Condition()
        {
            var result = mOnCondition?.Invoke();
            return result == null || result.Value;
        }

        public void Enter()
        {
            mOnEnter?.Invoke();
        }
        

        public void Update()
        {
            mOnUpdate?.Invoke();

        }

        public void FixedUpdate()
        {
            mOnFixedUpdate?.Invoke();
        }

        
        public void OnGUI()
        {
            mOnGUI?.Invoke();
        }

        public void Exit()
        {
            mOnExit?.Invoke();
        }
    }
#if UNITY_EDITOR
    [ClassAPI("10.FSM","FSM",0,"FSM")]
    [APIDescriptionCN("简易状态机")]
    [APIDescriptionEN("Simple FSM")]
    [APIExampleCode(@"
using UnityEngine;

namespace QFramework.Example
{
    public class IStateBasicUsageExample : MonoBehaviour
    {
        public enum States
        {
            A,
            B
        }

        public FSM<States> FSM = new FSM<States>();

        void Start()
        {
            FSM.OnStateChanged((previousState, nextState) =>
            {
                Debug.Log($""{previousState}=>{nextState}"");
            });

            FSM.State(States.A)
                .OnCondition(()=>FSM.CurrentStateId == States.B)
                .OnEnter(() =>
                {
                    Debug.Log(""Enter A"");
                })
                .OnUpdate(() =>
                {
                    
                })
                .OnFixedUpdate(() =>
                {
                    
                })
                .OnGUI(() =>
                {
                    GUILayout.Label(""State A"");
                    if (GUILayout.Button(""To State B""))
                    {
                        FSM.ChangeState(States.B);
                    }
                })
                .OnExit(() =>
                {
                    Debug.Log(""Exit A"");
                });

                FSM.State(States.B)
                    .OnCondition(() => FSM.CurrentStateId == States.A)
                    .OnGUI(() =>
                    {
                        GUILayout.Label(""State B"");
                        if (GUILayout.Button(""To State A""))
                        {
                            FSM.ChangeState(States.A);
                        }
                    });
            
                FSM.StartState(States.A);
            }

            private void Update()
            {
                FSM.Update();
            }

            private void FixedUpdate()
            {
                FSM.FixedUpdate();
            }

            private void OnGUI()
            {
                FSM.OnGUI();
            }

            private void OnDestroy()
            {
                FSM.Clear();
            }
        }
    }
}
// Enter A
// Exit A
// A=>B
// Enter B

// class state
using UnityEngine;

namespace QFramework.Example
{
    public class IStateClassExample : MonoBehaviour
    {

        public enum States
        {
            A,
            B,
            C
        }

        public FSM<States> FSM = new FSM<States>();

        public class StateA : AbstractState<States,IStateClassExample>
        {
            public StateA(FSM<States> fsm, IStateClassExample owner) : base(fsm, owner)
            {
            }

            protected override bool OnCondition()
            {
                return mFSM.CurrentStateId == States.B;
            }

            public override void OnGUI()
            {
                GUILayout.Label(""State A"");

                if (GUILayout.Button(""To State B""))
                {
                    mFSM.ChangeState(States.B);
                }
            }
        }
        
        
        public class StateB: AbstractState<States,IStateClassExample>
        {
            public StateB(FSM<States> fsm, IStateClassExample owner) : base(fsm, owner)
            {
            }

            protected override bool OnCondition()
            {
                return mFSM.CurrentStateId == States.A;
            }

            public override void OnGUI()
            {
                GUILayout.Label(""State B"");

                if (GUILayout.Button(""To State A""))
                {
                    mFSM.ChangeState(States.A);
                }
            }
        }

        private void Start()
        {
            FSM.AddState(States.A, new StateA(FSM, this));
            FSM.AddState(States.B, new StateB(FSM, this));

            // 支持和链式模式混用
            // FSM.State(States.C)
            //     .OnEnter(() =>
            //     {
            //
            //     });
            
            FSM.StartState(States.A);
        }

        private void OnGUI()
        {
            FSM.OnGUI();
        }

        private void OnDestroy()
        {
            FSM.Clear();
        }
    }
}
")]
#endif
    public class FSM<T>
    {
        protected Dictionary<T, IState> mStates = new Dictionary<T, IState>();

        public void AddState(T id, IState state)
        {
            mStates.Add(id,state);
        }
        
        
        public CustomState State(T t)
        {
            if (mStates.ContainsKey(t))
            {
                return mStates[t] as CustomState;
            }

            var state = new CustomState();
            mStates.Add(t, state);
            return state;
        }

        private IState mCurrentState;
        private T mCurrentStateId;

        public IState CurrentState => mCurrentState;
        public T CurrentStateId => mCurrentStateId;
        public T PreviousStateId { get; private set; }

        public long FrameCountOfCurrentState = 1;
        public float SecondsOfCurrentState = 0.0f;
        
        public void ChangeState(T t)
        {
            if (t.Equals(CurrentStateId)) return;
            
            if (mStates.TryGetValue(t, out var state))
            {
                if (mCurrentState != null && state.Condition())
                {
                    mCurrentState.Exit();
                    PreviousStateId = mCurrentStateId;
                    mCurrentState = state;
                    mCurrentStateId = t;
                    mOnStateChanged?.Invoke(PreviousStateId, CurrentStateId);
                    FrameCountOfCurrentState = 1;
                    SecondsOfCurrentState = 0.0f;
                    mCurrentState.Enter();
                }
            }
        }

        private Action<T, T> mOnStateChanged = (_, __) => { };
        
        public void OnStateChanged(Action<T, T> onStateChanged)
        {
            mOnStateChanged += onStateChanged;
        }

        public void StartState(T t)
        {
            if (mStates.TryGetValue(t, out var state))
            {
                PreviousStateId = t;
                mCurrentState = state;
                mCurrentStateId = t;
                FrameCountOfCurrentState = 0;
                SecondsOfCurrentState = 0.0f;
                state.Enter();
            }
        }

        public void FixedUpdate()
        {
            mCurrentState?.FixedUpdate();
        }

        public void Update()
        {
            mCurrentState?.Update();
            FrameCountOfCurrentState++;
            SecondsOfCurrentState += Time.deltaTime;
        }

        public void OnGUI()
        {
            mCurrentState?.OnGUI();
        }

        public void Clear()
        {
            mCurrentState = null;
            mCurrentStateId = default;
            mStates.Clear();
        }
    }
    
    public abstract class AbstractState<TStateId,TOwner> : IState
    {
        protected FSM<TStateId> mFSM;
        
        [Obsolete("please use mOwner,will removed in v1.1,请使用 mOwner,将在 v1.1 移除")]
        protected TOwner mTarget => mOwner;
        protected TOwner mOwner;

        public AbstractState(FSM<TStateId> fsm,TOwner owner)
        {
            mFSM = fsm;
            mOwner = owner;
        }


        bool IState.Condition()
        {
            return  OnCondition();;
        }

        void IState.Enter()
        {
            OnEnter();
        }

        void IState.Update()
        {
            OnUpdate();
        }

        void IState.FixedUpdate()
        {
            OnFixedUpdate();
        }

        public virtual void OnGUI()
        {
        }

        void IState.Exit()
        {
            OnExit();
        }

        protected virtual bool OnCondition() => true;

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {
            
        }

        protected virtual void OnFixedUpdate()
        {
            
        }

        protected virtual void OnExit()
        {
            
        }
    }
}