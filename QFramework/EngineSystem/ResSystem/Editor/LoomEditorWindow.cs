using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Threading;
using System;
using System.IO;
using System.Linq;

namespace QFramework.ResSystem
{
	public class LoomEditorWindow : EditorWindow
	{

		public int maxThreads = 8;

		private int numThreads;
		private int _count;

		private bool m_HasLoaded = false;

		private List<Action> _actions = new List<Action> ();
		private List<DelayedQueueItem> _delayed = new  List<DelayedQueueItem> ();

		private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem> ();
		private List<Action> _currentActions = new List<Action> ();

		public struct DelayedQueueItem
		{
			public float time;
			public Action action;
		}

		protected void QueueOnMainThread (Action action)
		{
			QueueOnMainThread (action, 0f);
		}

		protected void QueueOnMainThread (Action action, float time)
		{
			if (time != 0) {
				lock (_delayed) {
					_delayed.Add (new DelayedQueueItem { time = Time.time + time, action = action });
				}
			} else {
				lock (_actions) {
					_actions.Add (action);
				}
			}
		}

		protected Thread RunAsync (Action a)
		{
			while (numThreads >= maxThreads) {
				Thread.Sleep (1);
			}
			Interlocked.Increment (ref numThreads);
			ThreadPool.QueueUserWorkItem (RunAction, a);
			return null;
		}

		private void RunAction (object action)
		{
			try {
				((Action)action) ();
			} catch {
			} finally {
				Interlocked.Decrement (ref numThreads);
			}
		}

		protected virtual void Start ()
		{
			m_HasLoaded = true;

		}

		protected virtual void Update ()
		{
			if (m_HasLoaded == false)
				Start ();

			lock (_actions) {
				_currentActions.Clear ();
				_currentActions.AddRange (_actions);
				_actions.Clear ();
			}
			foreach (var a in _currentActions) {
				a ();
			}
			lock (_delayed) {
				_currentDelayed.Clear ();
				_currentDelayed.AddRange (_delayed.Where (d => d.time <= Time.time));
				foreach (var item in _currentDelayed)
					_delayed.Remove (item);
			}
			foreach (var delayed in _currentDelayed) {
				delayed.action ();
			}
		}
	}
}