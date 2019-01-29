using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Editor
{
	public interface IPackageKitView
	{
		IQFrameworkContainer Container { get; set; }
		/// <summary>
		/// 1 after 0
		/// </summary>
		int RenderOrder { get;}
		
		bool Ignore { get; }
		
		bool Enabled { get;}
		
		void Init(IQFrameworkContainer container);

		void OnGUI();
	}
}