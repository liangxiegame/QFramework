using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class ActionKitFSMTransitionTable : Table<ActionKitFSMTransition>
    {
        public TableIndex<Type, ActionKitFSMTransition> SrcStateIndex =
            new TableIndex<Type, ActionKitFSMTransition>(t => t.SrcStateType);

        public TableIndex<Type, ActionKitFSMTransition> TypeIndex =
            new TableIndex<Type, ActionKitFSMTransition>(t => t.GetType());

        protected override void OnAdd(ActionKitFSMTransition item)
        {
            SrcStateIndex.Add(item);
            TypeIndex.Add(item);
        }

        protected override void OnRemove(ActionKitFSMTransition item)
        {
            SrcStateIndex.Remove(item);
            TypeIndex.Remove(item);
        }

        protected override void OnClear()
        {
            SrcStateIndex.Clear();
            TypeIndex.Clear();
        }

        public override IEnumerator<ActionKitFSMTransition> GetEnumerator()
        {
            return SrcStateIndex.Dictionary.SelectMany(src => src.Value).GetEnumerator();
        }

        protected override void OnDispose()
        {
            SrcStateIndex.Dispose();
            TypeIndex.Dispose();
        }
    }

    public class ActionKitFSM
    {
        public ActionKitFSMState CurrentState { get; private set; }

        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        private Dictionary<Type, ActionKitFSMState> mStates = new Dictionary<Type, ActionKitFSMState>();
        private ActionKitFSMTransitionTable mTrasitionTable = new ActionKitFSMTransitionTable();


        public void AddState(ActionKitFSMState state)
        {
            mStates.Add(state.GetType(), state);
        }

        public void AddTransition(ActionKitFSMTransition transition)
        {
            mTrasitionTable.Add(transition);
        }

        public bool HandleEvent<TTransition>()
        {
            var transition = mTrasitionTable.TypeIndex.Get(typeof(TTransition)).First();

            if (transition.SrcStateType == CurrentState.GetType())
            {
                CurrentState.Exit();
                CurrentState = mStates[transition.DstStateType];
                CurrentState.Enter();
                return true;
            }

            return false;
        }
        
        public void ChangeState<TState>()
        {
            CurrentState.Exit();
            CurrentState = mStates[typeof(TState)];
            CurrentState.Enter();
        }

        public void StartState<T>()
        {
            CurrentState = mStates[typeof(T)];
            CurrentState.Enter();
        }
    }

    public class ActionKitFSMState
    {
        public void Enter()
        {
            OnEnter();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }
    }

    public class ActionKitFSMState<T> : ActionKitFSMState
    {
        public ActionKitFSMState(T target)
        {
            mTarget = target;
        }

        protected T mTarget;
    }



    public class ActionKitFSMTransition
    {
        public virtual Type SrcStateType { get; set; }

        public virtual Type DstStateType { get; set; }
    }

    public class ActionKitFSMTransition<TSrcState, TDstState> : ActionKitFSMTransition
    {
        private Type mSrcStateType = typeof(TSrcState);
        private Type mDstStateType = typeof(TDstState);

        public override Type SrcStateType
        {
            get { return mSrcStateType; }
        }

        public override Type DstStateType
        {
            get { return mDstStateType; }
        }
    }
}