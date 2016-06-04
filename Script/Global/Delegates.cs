using UnityEngine;
using System.Collections;

namespace QFramework {
	/// <summary>
	/// 返回空类型的回调定义
	/// </summary>
	public class VoidDelegate{

		public delegate void WithVoid();

		public delegate void WithParams(params object[] paramList);
	}

}
