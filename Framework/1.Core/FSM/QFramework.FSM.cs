using System;
using System.Collections.Generic;

namespace QFramework
{
	/// <summary>
	/// FSM 基于枚举的状态机
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
		public delegate void FSMOnStateChanged(params object[] param);

		/// <summary>
		/// QFSM state.
		/// </summary>
		public class FSMState<TName>
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

	/// <summary>
	/// QFSM lite.
	/// 基于字符串的状态机
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