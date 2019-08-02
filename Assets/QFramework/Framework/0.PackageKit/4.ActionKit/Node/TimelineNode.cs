/****************************************************************************
 * Copyright (c) 2017 liangxie
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
	using System.Linq;
	using System.Collections.Generic;
	using System;

	/// <summary>
	/// 时间轴执行节点
	/// </summary>
	public class TimelineNode : NodeAction
	{
		private float mCurTime = 0;

		public System.Action OnTimelineBeganCallback
		{
			get { return OnBeganCallback; }
			set { OnBeganCallback = value; }
		}

		public System.Action OnTimelineEndedCallback
		{
			get { return OnEndedCallback; }
			set { OnEndedCallback = value; }
		}

		public Action<string> OnKeyEventsReceivedCallback = null;

		public class TimelinePair
		{
			public float Time;
			public IAction Node;

			public TimelinePair(float time, IAction node)
			{
				Time = time;
				Node = node;
			}
		}

		/// <summary>
		/// refator 2 one list? all in one list;
		/// </summary>
		public Queue<TimelinePair> TimelineQueue = new Queue<TimelinePair>();

		protected override void OnReset()
		{
			mCurTime = 0.0f;

			foreach (var timelinePair in TimelineQueue)
			{
				timelinePair.Node.Reset();
			}
		}

		protected override void OnExecute(float dt)
		{
			mCurTime += dt;

			foreach (var pair in TimelineQueue.Where(pair => pair.Time < mCurTime && !pair.Node.Finished))
			{
				if (pair.Node.Execute(dt))
				{
					Finished = TimelineQueue.Where(timetinePair => !timetinePair.Node.Finished).Count() == 0;
				}
			}
		}

		public TimelineNode(params TimelinePair[] pairs)
		{
			foreach (var pair in pairs)
			{
				TimelineQueue.Enqueue(pair);
			}
		}

		public void Append(TimelinePair pair)
		{
			TimelineQueue.Enqueue(pair);
		}

		public void Append(float time, IAction node)
		{
			TimelineQueue.Enqueue(new TimelinePair(time, node));
		}

		protected override void OnDispose()
		{
			foreach (var timelinePair in TimelineQueue)
			{
				timelinePair.Node.Dispose();
			}

			TimelineQueue.Clear();
			TimelineQueue = null;
		}
	}
}