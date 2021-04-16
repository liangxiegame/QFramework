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

using UnityEngine;

namespace QFramework
{
    [System.Serializable]
    public class AIActionsList : ReorderableArray<AIAction>
    {
    }
    [System.Serializable]
    public class AITransitionsList : ReorderableArray<AITransition>
    {
    }

    /// <summary>
    /// A State is a combination of one or more actions, and one or more transitions. An example of a state could be "_patrolling until an enemy gets in range_".
    /// </summary>
    [System.Serializable]
    public class AIState 
    {
        /// the name of the state (will be used as a reference in Transitions
        public string StateName;

        [Reorderable(null, "Action", null)]
        public AIActionsList Actions;
        [Reorderable(null, "Transition", null)]
        public AITransitionsList Transitions;/*

        /// a list of actions to perform in this state
        public List<AIAction> Actions;
        /// a list of transitions to evaluate to exit this state
        public List<AITransition> Transitions;*/

        protected AIBrain mBrain;

        /// <summary>
        /// Sets this state's brain to the one specified in parameters
        /// </summary>
        /// <param name="brain"></param>
        public virtual void SetBrain(AIBrain brain)
        {
            mBrain = brain;
        }
                		
        /// <summary>
        /// Updates the state, performing actions and testing transitions
        /// </summary>
        public virtual void UpdateState()
        {
            PerformActions();
            EvaluateTransitions();
        }

        /// <summary>
        /// On enter state we pass that info to our actions and decisions
        /// </summary>
        public virtual void EnterState()
        {
            foreach (AIAction action in Actions)
            {
                action.OnEnterState();
            }
            foreach (AITransition transition in Transitions)
            {
                if (transition.Decision != null)
                {
                    transition.Decision.OnEnterState();
                }
            }
        }

        /// <summary>
        /// On exit state we pass that info to our actions and decisions
        /// </summary>
        public virtual void ExitState()
        {
            foreach (AIAction action in Actions)
            {
                action.OnExitState();
            }
            foreach (AITransition transition in Transitions)
            {
                if (transition.Decision != null)
                {
                    transition.Decision.OnExitState();
                }
            }
        }

        /// <summary>
        /// Performs this state's actions
        /// </summary>
        protected virtual void PerformActions()
        {
            if (Actions.Count == 0) { return; }
            for (int i=0; i<Actions.Count; i++) 
            {
                if (Actions[i] != null)
                {
                    Actions[i].PerformAction();
                }
                else
                {
                    Log.E("An action in " + mBrain.gameObject.name + " is null.");
                }
            }
        }
        
        /// <summary>
        /// Tests this state's transitions
        /// </summary>
        protected virtual void EvaluateTransitions()
        {
            if (Transitions.Count == 0) { return; }
            for (int i = 0; i < Transitions.Count; i++) 
            {
                if (Transitions[i].Decision != null)
                {
                    if (Transitions[i].Decision.Decide())
                    {
                        if (Transitions[i].TrueState != "")
                        {
                            mBrain.TransitionToState(Transitions[i].TrueState);
                        }
                    }
                    else
                    {
                        if (Transitions[i].FalseState != "")
                        {
                            mBrain.TransitionToState(Transitions[i].FalseState);
                        }
                    }
                }                
            }
        }        
	}
}
