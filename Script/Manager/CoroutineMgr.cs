using UnityEngine;
using System.Collections;

/// <summary>
/// 协程管理

/// 分两种
/// 1.需要全局调用的协程 App （可单独调用,可全部调用)
/// 2.需要自身管理的协程 Go
/// </summary>
namespace QFramework {
	public class CoroutineMgr : QSingleton<CoroutineMgr> {

		protected CoroutineMgr() {}



		public Coroutine StartCoroutine(IEnumerator routine) {
			return App.Instance ().StartCoroutine (routine);
		}
	}
}
