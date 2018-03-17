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

namespace QFramework
{
	using System.Collections.Generic;

	public class QFSMState
	{
		public QFSMState(ushort stateName)
		{
			Name = stateName;
		}

		public ushort Name; // 字符串

		public virtual void OnEnter()
		{
		} // 进入状态(逻辑)

		public virtual void OnExit()
		{
		} // 离开状态(逻辑)

		/// <summary>
		/// translation for name
		/// </summary>
		public Dictionary<ushort, QFSMTranslation> TranslationDict = new Dictionary<ushort, QFSMTranslation>();
	}

	/// <summary>
	/// 跳转类
	/// </summary>
	public class QFSMTranslation
	{
		public QFSMState FromState;
		public ushort EventName;
		public QFSMState ToState;

		public QFSMTranslation(QFSMState fromState, ushort eventName, QFSMState toState)
		{
			FromState = fromState;
			ToState = toState;
			EventName = eventName;
		}
	}

	public class QFSM
	{
		private QFSMState mCurState;

		public QFSMState State
		{
			get { return mCurState; }
		}

		/// <summary>
		/// The m state dict.
		/// </summary>
		private readonly Dictionary<ushort, QFSMState> mStateDict = new Dictionary<ushort, QFSMState>();

		/// <summary>
		/// Adds the state.
		/// </summary>
		/// <param name="state">State.</param>
		public void AddState(QFSMState state)
		{
			mStateDict.Add(state.Name, state);
		}


		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="translation">Translation.</param>
		public void AddTranslation(QFSMTranslation translation)
		{
			mStateDict[translation.FromState.Name].TranslationDict.Add(translation.EventName, translation);
		}


		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="fromState">From state.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="toState">To state.</param>
		public void AddTranslation(QFSMState fromState, ushort eventName, QFSMState toState)
		{
			mStateDict[fromState.Name].TranslationDict.Add(eventName, new QFSMTranslation(fromState, eventName, toState));
		}

		/// <summary>
		/// Start the specified startState.
		/// </summary>
		/// <param name="startState">Start state.</param>
		public void Start(QFSMState startState)
		{
			mCurState = startState;
			mCurState.OnEnter();
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		public void HandleEvent(ushort eventName)
		{
			if (mCurState != null && mStateDict[mCurState.Name].TranslationDict.ContainsKey(eventName))
			{
				var tempTranslation = mStateDict[mCurState.Name].TranslationDict[eventName];
				tempTranslation.FromState.OnExit();
				mCurState = tempTranslation.ToState;
				tempTranslation.ToState.OnEnter();
			}
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			mStateDict.Clear();
		}
	}
}