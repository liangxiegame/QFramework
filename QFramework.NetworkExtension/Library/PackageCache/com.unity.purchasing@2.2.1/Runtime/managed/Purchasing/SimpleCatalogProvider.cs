using System;
using System.Collections.Generic;
using UnityEngine.Purchasing.Extension;

namespace UnityEngine.Purchasing
{
	internal class SimpleCatalogProvider : ICatalogProvider
	{
		private Action<Action<HashSet<ProductDefinition>>> m_Func;

		internal SimpleCatalogProvider (Action<Action<HashSet<ProductDefinition>>> func)
		{
			m_Func = func;
		}

		public void FetchProducts (Action<HashSet<ProductDefinition>> callback)
		{
			if (m_Func != null) {
				m_Func (callback);
			}
		}
	}
}
