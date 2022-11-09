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
            FSM.State(States.A)
                .OnCondition(()=>FSM.CurrentStateId == States.B)
                .OnEnter(() =>
                {
                    Debug.Log("Enter A");
                })
                .OnUpdate(() =>
                {
                    
                })
                .OnFixedUpdate(() =>
                {
                    
                })
                .OnGUI(() =>
                {
                    GUILayout.Label("State A");
                    if (GUILayout.Button("To State B"))
                    {
                        FSM.ChangeState(States.B);
                    }
                })
                .OnExit(() =>
                {
                    Debug.Log("Enter B");

                });

            FSM.State(States.B)
                .OnCondition(() => FSM.CurrentStateId == States.A)
                .OnGUI(() =>
                {
                    GUILayout.Label("State B");
                    if (GUILayout.Button("To State A"))
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