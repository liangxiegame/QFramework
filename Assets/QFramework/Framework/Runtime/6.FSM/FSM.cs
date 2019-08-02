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

namespace QF
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// FSM
	/// </summary>
	public class FSM<TStateEnum,TEventEnum> : IDisposable
	{

		private Action<TStateEnum, TStateEnum> mOnStateChanged = null;
		
		public FSM(Action<TStateEnum,TStateEnum> onStateChanged = null)
		{
			mOnStateChanged = onStateChanged;
		}
		
		/// <summary>
		/// FSM onStateChagned.
		/// </summary>
		public delegate void FSMOnStateChagned(params object[] param);

		/// <summary>
		/// QFSM state.
		/// </summary>
		class FSMState<TName>
		{
			public TName Name;

			public FSMState(TName name)
			{
				Name = name;
			}

			/// <summary>
			/// The translation dict.
			/// </summary>
			public readonly Dictionary<TEventEnum, FSMTranslation<TName, TEventEnum>> TranslationDict =
				new Dictionary<TEventEnum, FSMTranslation<TName, TEventEnum>>();
		}

		/// <summary>
		/// Translation 
		/// </summary>
		public class FSMTranslation<TStateName, KEventName>
		{
			public TStateName FromState;
			public KEventName Name;
			public TStateName ToState;
			public Action<object[]> OnTranslationCallback; // 回调函数

			public FSMTranslation(TStateName fromState, KEventName name, TStateName toState,
				Action<object[]> onStateChagned)
			{
				FromState = fromState;
				ToState = toState;
				Name = name;
				OnTranslationCallback = onStateChagned;
			}
		}

		/// <summary>
		/// The state of the m current.
		/// </summary>
		TStateEnum mCurState;

		public TStateEnum State
		{
			get { return mCurState; }
		}

		/// <summary>
		/// The m state dict.
		/// </summary>
		Dictionary<TStateEnum, FSMState<TStateEnum>> mStateDict = new Dictionary<TStateEnum, FSMState<TStateEnum>>();

		/// <summary>
		/// Adds the state.
		/// </summary>
		/// <param name="name">Name.</param>
		private void AddState(TStateEnum name)
		{
			mStateDict[name] = new FSMState<TStateEnum>(name);
		}

		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="fromState">From state.</param>
		/// <param name="name">Name.</param>
		/// <param name="toState">To state.</param>
		/// <param name="onStateChagned">Callfunc.</param>
		public void AddTransition(TStateEnum fromState, TEventEnum name, TStateEnum toState, Action<object[]> onStateChagned = null  )
		{
			if (!mStateDict.ContainsKey(fromState))
			{
				AddState(fromState );
			}

			if (!mStateDict.ContainsKey(toState))
			{
				AddState(toState);
			}

			mStateDict[fromState].TranslationDict[name] = new FSMTranslation<TStateEnum,TEventEnum>(fromState, name, toState, onStateChagned);
		}

		/// <summary>
		/// Start the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public void Start(TStateEnum name)
		{
			mCurState = name;
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="param">Parameter.</param>
		public void HandleEvent(TEventEnum name, params object[] param)
		{
			if (mCurState != null && mStateDict[mCurState].TranslationDict.ContainsKey(name))
			{
				var tempTranslation = mStateDict[mCurState].TranslationDict[name];

				if (tempTranslation.OnTranslationCallback != null)
				{
					tempTranslation.OnTranslationCallback.Invoke(param);
				}

				if (mOnStateChanged != null)
				{
					mOnStateChanged.Invoke(mCurState, tempTranslation.ToState);
				}

				mCurState = tempTranslation.ToState;
			}
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			foreach (var keyValuePair in mStateDict)
			{
				foreach (var translationDictValue in keyValuePair.Value.TranslationDict.Values)
				{
					translationDictValue.OnTranslationCallback = null;
				}
				
				keyValuePair.Value.TranslationDict.Clear();
			}
		
			mStateDict.Clear();
		}

		public void Dispose()
		{
			Clear();
		}
	}
}