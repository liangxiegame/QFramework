using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace QFramework {
	/// <summary>
	/// 返回空类型的回调定义
	/// </summary>
	public class VoidDelegate{

		public delegate void WithVoid();

		public delegate void WithGo(GameObject go);

		public delegate void WithParams(params object[] paramList);

		public delegate void WithEvent(BaseEventData data);
	}

}
