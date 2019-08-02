/****************************************************************************
 * Copyright (c) 2017 ~ 2018.8 liangxie
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

namespace QF.Action
{
	using System.Collections.Generic;
	
	/// <summary>
	/// 并发执行的协程
	/// </summary>
	public class SpawnNode :NodeAction 
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
}