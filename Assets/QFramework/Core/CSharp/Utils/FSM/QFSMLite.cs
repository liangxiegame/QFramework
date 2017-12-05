/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using System.Collections.Generic;

	/// <summary>
	/// 教程地址:http://liangxiegame.com/post/4/
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
			private string mName;

			public QFSMState(string name)
			{
				mName = name;
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

		public string State { get; private set; }

		/// <summary>
		/// The m state dict.
		/// </summary>
		private readonly Dictionary<string, QFSMState> mStateDict = new Dictionary<string, QFSMState>();

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
			State = name;
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="param">Parameter.</param>
		public void HandleEvent(string name, params object[] param)
		{
			if (State != null && mStateDict[State].TranslationDict.ContainsKey(name))
			{
				QFSMTranslation tempTranslation = mStateDict[State].TranslationDict[name];
				tempTranslation.OnTranslationCallback(param);
				State = tempTranslation.ToState;
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