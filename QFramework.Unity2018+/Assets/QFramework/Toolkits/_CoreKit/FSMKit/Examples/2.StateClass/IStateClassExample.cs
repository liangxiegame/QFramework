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
            public StateA(FSM<States> fsm, IStateClassExample target) : base(fsm, target)
            {
            }

            protected override bool OnCondition()
            {
                return mFSM.CurrentStateId == States.B;
            }

            public override void OnGUI()
            {
                GUILayout.Label("State A");

                if (GUILayout.Button("To State B"))
                {
                    mFSM.ChangeState(States.B);
                }
            }
        }
        
        
        public class StateB: AbstractState<States,IStateClassExample>
        {
            public StateB(FSM<States> fsm, IStateClassExample target) : base(fsm, target)
            {
            }

            protected override bool OnCondition()
            {
                return mFSM.CurrentStateId == States.A;
            }

            public override void OnGUI()
            {
                GUILayout.Label("State B");

                if (GUILayout.Button("To State A"))
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
