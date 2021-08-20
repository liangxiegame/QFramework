/****************************************************************************
 * Copyright (c) 2021.3 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public class ActionKitFSM
    {
        public ActionKitFSMState CurrentState { get; private set; }
        public Type PreviousStateType { get; private set; }

        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public void FixedUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.FixedUpdate();
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

        public bool HandleEvent<TTransition>() where TTransition : ActionKitFSMTransition
        {
            foreach (var transition in mTrasitionTable.TypeIndex.Get(typeof(TTransition)))
            {
                if (transition.SrcStateTypes.Contains(CurrentState.GetType()))
                {
                    var currentState = CurrentState;
                    var nextState = mStates[transition.DstStateType];
                    CurrentState.Exit();
                    transition.OnTransition(currentState, nextState);
                    CurrentState = nextState;
                    CurrentState.Enter();
                    return true;
                }
            }

            return false;
        }

        public void ChangeState<TState>() where TState : ActionKitFSMState
        {
            ChangeState(typeof(TState));
        }

        public void ChangeState(Type stateType)
        {
            if (CurrentState.GetType() != stateType)
            {
                PreviousStateType = CurrentState.GetType();
                CurrentState.Exit();
                CurrentState = mStates[stateType];
                CurrentState.Enter();
            }
        }

        public void BackToPreviousState()
        {
            ChangeState(PreviousStateType);
        }

        public void StartState<T>()
        {
            CurrentState = mStates[typeof(T)];
            CurrentState.Enter();
        }
    }
}