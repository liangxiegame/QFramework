/****************************************************************************
 * Copyright (c) 2021.4  liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class AIBrain : MonoBehaviour
    {
          /// the collection of states
        public List<AIState> States;
        /// whether or not this brain is active
        public bool BrainActive = true;
        /// this brain's current state
        public AIState CurrentState { get; protected set; }
        [ReadOnly]
        /// the time we've spent in the current state
        public float TimeInThisState;
        [ReadOnly]
        /// the current target
        public Transform Target;

        protected AIDecision[] _decisions;

        /// <summary>
        /// On awake we set our brain for all states
        /// </summary>
        protected virtual void Awake()
        {
            foreach (AIState state in States)
            {
                state.SetBrain(this);
            }
            _decisions = this.gameObject.GetComponents<AIDecision>();
        }

        /// <summary>
        /// On Start we set our first state
        /// </summary>
        protected virtual void Start()
        {
            if (States.Count > 0)
            {
                CurrentState = States[0];
                CurrentState.EnterState();
            }            
        }

        /// <summary>
        /// Every frame we update our current state
        /// </summary>
        protected virtual void Update()
        {
            if (!BrainActive || CurrentState == null)
            {
                return;
            }
            CurrentState.UpdateState();
            TimeInThisState += Time.deltaTime;
        }

        /// <summary>
        /// Transitions to the specified state, trigger exit and enter states events
        /// </summary>
        /// <param name="newStateName"></param>
        public virtual void TransitionToState(string newStateName)
        {
            if (newStateName != CurrentState.StateName)
            {
                CurrentState.ExitState();
                OnExitState();

                CurrentState = FindState(newStateName);
                if (CurrentState != null)
                {
                    CurrentState.EnterState();
                }                
            }
        }
        
        /// <summary>
        /// When exiting a state we reset our time counter
        /// </summary>
        protected virtual void OnExitState()
        {
            TimeInThisState = 0f;
        }

        /// <summary>
        /// Initializes all decisions
        /// </summary>
        protected virtual void InitializeDecisions()
        {
            foreach(AIDecision decision in _decisions)
            {
                decision.Initialization();
            }
        }

        /// <summary>
        /// Returns a state based on the specified state name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        protected AIState FindState(string stateName)
        {
            foreach (AIState state in States)
            {
                if (state.StateName == stateName)
                {
                    return state;
                }
            }
            Log.E("You're trying to transition to state '" + stateName + "' in " + this.gameObject.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
            return null;
        }
    }
}