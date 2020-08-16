using System;
using UnityEngine;

namespace QFramework
{
	public class Core :  AbstractArchitectureConfig<IFrameworkCommand,Core>
	{
		[Obsolete("不可以自行创建 Core 的实例")]
		public Core() {}

		[RuntimeInitializeOnLoadMethod]
		static void InitOnLoad()
		{
			var config = mConfig;
			Debug.Log("QFramework Core Initialized:" + config);
		}

		protected override void OnSystemConfig(IQFrameworkContainer systemLayer)
		{
			
		}

		protected override void OnModelConfig(IQFrameworkContainer modelLayer)
		{
			
		}

		protected override void OnUtilityConfig(IQFrameworkContainer utilityLayer)
		{
			utilityLayer.RegisterInstance<IJsonSerializeUtility>(new DefaultJsonSerializeUtility());
		}
	}
}