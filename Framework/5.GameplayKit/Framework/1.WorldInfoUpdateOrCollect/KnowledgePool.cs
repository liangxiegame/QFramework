using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
	public class KnowledgePool
	{
		public Dictionary<string, Knowledge> Knowledges = new Dictionary<string, Knowledge>();

		public Knowledge<T> Get<T>(string key)
		{
			return Knowledges[key] as Knowledge<T>;
		}

	}


	public abstract class Knowledge
	{
		public object Value { get; set; }
	}

	public class Knowledge<T> : Knowledge
	{
		public T GetValue()
		{
			return (T) Value;
		}
	}
}
