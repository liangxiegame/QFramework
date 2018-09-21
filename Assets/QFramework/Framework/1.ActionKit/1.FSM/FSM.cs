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
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// FSM
	/// </summary>
	public class FSM<TStateName,KEventName> : IDisposable
	{

		private Action<TStateName, TStateName> mOnStateChanged = null;
		
		public FSM(Action<TStateName,TStateName> onStateChanged = null)
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
		class FSMState<TStateName>
		{
			public TStateName Name;

			public FSMState(TStateName name)
			{
				Name = name;
			}

			/// <summary>
			/// The translation dict.
			/// </summary>
			public readonly Dictionary<KEventName, FSMTranslation<TStateName, KEventName>> TranslationDict =
				new Dictionary<KEventName, FSMTranslation<TStateName, KEventName>>();
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
		TStateName mCurState;

		public TStateName State
		{
			get { return mCurState; }
		}

		/// <summary>
		/// The m state dict.
		/// </summary>
		Dictionary<TStateName, FSMState<TStateName>> mStateDict = new Dictionary<TStateName, FSMState<TStateName>>();

		/// <summary>
		/// Adds the state.
		/// </summary>
		/// <param name="name">Name.</param>
		private void AddState(TStateName name)
		{
			mStateDict[name] = new FSMState<TStateName>(name);
		}

		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="fromState">From state.</param>
		/// <param name="name">Name.</param>
		/// <param name="toState">To state.</param>
		/// <param name="onStateChagned">Callfunc.</param>
		public void AddTransition(TStateName fromState, KEventName name, TStateName toState, Action<object[]> onStateChagned = null  )
		{
			if (!mStateDict.ContainsKey(fromState))
			{
				AddState(fromState );
			}

			if (!mStateDict.ContainsKey(toState))
			{
				AddState(toState);
			}

			mStateDict[fromState].TranslationDict[name] = new FSMTranslation<TStateName,KEventName>(fromState, name, toState, onStateChagned);
		}

		/// <summary>
		/// Start the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public void Start(TStateName name)
		{
			mCurState = name;
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="param">Parameter.</param>
		public void HandleEvent(KEventName name, params object[] param)
		{
			if (mCurState != null && mStateDict[mCurState].TranslationDict.ContainsKey(name))
			{
				var tempTranslation = mStateDict[mCurState].TranslationDict[name];
				tempTranslation.OnTranslationCallback.InvokeGracefully(param);
				mOnStateChanged.InvokeGracefully(mCurState, tempTranslation.ToState);
				mCurState = tempTranslation.ToState;
			}
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			mStateDict.Values.ForEach(state =>
			{
				state.TranslationDict.Values.ForEach(translation => { translation.OnTranslationCallback = null; });
				state.TranslationDict.Clear();
			});

			mStateDict.Clear();
		}

		public void Dispose()
		{
			Clear();
		}
	}
}