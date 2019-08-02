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
	using System.Collections.Generic;

	/// <summary>
	/// QFSM lite.
	/// </summary>
	public class QFSMLite
	{
		/// <summary>
		/// FSM callfunc.
		/// </summary>
		public delegate void FSMCallfunc(params object[] param);

		/// <summary>
		/// QFSM state.
		/// </summary>
		class QFSMState
		{
			public string Name;

			public QFSMState(string name)
			{
				Name = name;
			}

			/// <summary>
			/// The translation dict.
			/// </summary>
			public readonly Dictionary<string, QFSMTranslation> TranslationDict = new Dictionary<string, QFSMTranslation>();
		}

		/// <summary>
		/// Translation 
		/// </summary>
		public class QFSMTranslation
		{
			public string FromState;
			public string Name;
			public string ToState;
			public FSMCallfunc OnTranslationCallback; // 回调函数

			public QFSMTranslation(string fromState, string name, string toState, FSMCallfunc onTranslationCallback)
			{
				FromState = fromState;
				ToState = toState;
				Name = name;
				OnTranslationCallback = onTranslationCallback;
			}
		}

		/// <summary>
		/// The state of the m current.
		/// </summary>
		string mCurState;

		public string State
		{
			get { return mCurState; }
		}

		/// <summary>
		/// The m state dict.
		/// </summary>
		Dictionary<string, QFSMState> mStateDict = new Dictionary<string, QFSMState>();

		/// <summary>
		/// Adds the state.
		/// </summary>
		/// <param name="name">Name.</param>
		public void AddState(string name)
		{
			mStateDict[name] = new QFSMState(name);
		}

		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="fromState">From state.</param>
		/// <param name="name">Name.</param>
		/// <param name="toState">To state.</param>
		/// <param name="callfunc">Callfunc.</param>
		public void AddTranslation(string fromState, string name, string toState, FSMCallfunc callfunc)
		{
			mStateDict[fromState].TranslationDict[name] = new QFSMTranslation(fromState, name, toState, callfunc);
		}

		/// <summary>
		/// Start the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public void Start(string name)
		{
			mCurState = name;
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="param">Parameter.</param>
		public void HandleEvent(string name, params object[] param)
		{
			if (mCurState != null && mStateDict[mCurState].TranslationDict.ContainsKey(name))
			{
				var tempTranslation = mStateDict[mCurState].TranslationDict[name];
				tempTranslation.OnTranslationCallback(param);
				mCurState = tempTranslation.ToState;
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