/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework
{
    using UnityEngine;
    
    using System;

    public abstract class ExecuteNodeChain : ExecuteNode, IExecuteNodeChain, IDisposeWhen
    {
        public MonoBehaviour Executer { get; set; }

        protected abstract ExecuteNode mNode { get; }

        public abstract IExecuteNodeChain Append(IExecuteNode node);

        protected override void OnBegin()
        {
            base.OnBegin();

            if (mDisposeWhenOnDestroyed)
            {
                this.AddTo(Executer);
            }
        }

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition.InvokeGracefully())
            {
                Finished = true;
            }
            else
            {
                Finished = mNode.Execute(dt);
            }

            if (Finished && mDisposeWhenFinished)
            {
                Dispose();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Executer = null;
            mDisposeWhenCondition = false;
            mDisposeWhenFinished = false;
            mDisposeWhenOnDestroyed = false;
            mDisposeCondition = null;
            mOnDisposedEvent.InvokeGracefully();
            mOnDisposedEvent = null;
        }

        public IDisposeWhen Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }

        private bool mDisposeWhenOnDestroyed = false;
        private bool mDisposeWhenFinished = true;
        private bool mDisposeWhenCondition = false;
        private Func<bool> mDisposeCondition;
        private Action mOnDisposedEvent = null;

        public IDisposeEventRegister DisposeWhenGameObjDestroyed()
        {
            mDisposeWhenFinished = false;
            mDisposeWhenOnDestroyed = true;
            return this;
        }

        /// <summary>
        /// Default
        /// </summary>
        /// <returns></returns>
        public IDisposeEventRegister DisposeWhenFinished()
        {
            mDisposeWhenFinished = true;
            return this;
        }

        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenFinished = true;
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }

        public void OnDisposed(Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }
    }
}