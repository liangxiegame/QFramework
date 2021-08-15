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
    /// <summary>
    /// Decisions are components that will be evaluated by transitions, every frame, and will return true or false. Examples include time spent in a state, distance to a target, or object detection within an area.  
    /// </summary>
    public abstract class AIDecision : MonoBehaviour
    {
        /// Decide will be performed every frame while the Brain is in a state this Decision is in. Should return true or false, which will then determine the transition's outcome.
        public abstract bool Decide();

        protected AIBrain mBrain;

        /// <summary>
        /// On Start we initialize our Decision
        /// </summary>
        protected virtual void Start()
        {
            mBrain = this.gameObject.GetComponent<AIBrain>();
            Initialization();
        }
        
        /// <summary>
        /// Meant to be overridden, called when the game starts
        /// </summary>
        public virtual void Initialization()
        {

        }

        /// <summary>
        /// Meant to be overridden, called when the Brain enters a State this Decision is in
        /// </summary>
        public virtual void OnEnterState()
        {

        }

        /// <summary>
        /// Meant to be overridden, called when the Brain exits a State this Decision is in
        /// </summary>
        public virtual void OnExitState()
        {

        }
    }
}