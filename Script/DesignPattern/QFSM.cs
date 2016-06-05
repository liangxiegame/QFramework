using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 状态机实现
/// </summary>
public class QFSM {
	// 定义函数指针类型
	public delegate void FSMCallfunc();

	/// <summary>
	/// 状态类
	/// </summary>
	class QState 
	{		
		public string name;
		
		public QState(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// 存储事件对应的条转
		/// </summary>
		public Dictionary <string,FSMTranslation> TranslationDict = new Dictionary<string,FSMTranslation>();
	}

	/// <summary>
	/// 跳转类
	/// </summary>
	public class FSMTranslation
	{
		public string fromState;
		public string name;
		public string toState;
		public FSMCallfunc callfunc;	// 回调函数

		public FSMTranslation(string fromState,string name, string toState,FSMCallfunc callfunc)
		{
			this.fromState = fromState;
			this.toState   = toState;
			this.name = name;
			this.callfunc = callfunc;
		}
	}
		
	// 当前状态
	private string mCurState;

	public string State {
		get {
			return mCurState;
		}
	}

	// 状态
	Dictionary <string,QState> StateDict = new Dictionary<string,QState>();

	/// <summary>
	/// 添加状态
	/// </summary>
	/// <param name="state">State.</param>
	public void AddState(string name)
	{
		StateDict [name] = new QState(name);
	}
		
	/// <summary>
	/// 添加条转
	/// </summary>
	/// <param name="translation">Translation.</param>
	public void AddTranslation(string fromState,string name,string toState,FSMCallfunc callfunc)
	{
		StateDict [fromState].TranslationDict [name] = new FSMTranslation (fromState, name, toState, callfunc);
	}

	/// <summary>
	/// 启动状态机
	/// </summary>
	/// <param name="state">State.</param>
	public void Start(string name)
	{
		mCurState = name;
	}


	/// <summary>
	/// 处理事件
	/// </summary>
	/// <param name="name">Name.</param>
	public void HandleEvent(string name)
	{	
		if (mCurState != null && StateDict[mCurState].TranslationDict.ContainsKey(name)) {
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			FSMTranslation tempTranslation = StateDict [mCurState].TranslationDict [name];
			tempTranslation.callfunc ();
			mCurState =  tempTranslation.toState;

			watch.Stop ();
		}
	}

	public void Clear()
	{
		StateDict.Clear ();
	}
}
