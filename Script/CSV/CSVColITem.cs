using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {
	public class CSVColItem {

		public BASIC_TYPE type;
		public string Head;
		public CSVValue[] values;
		public Dictionary<string,int> indexForValue = new Dictionary<string, int>();

		public CSVColItem(string head,BASIC_TYPE type,int itemCount)
		{
			Head = head;
			this.type = type;
			values = new CSVValue[itemCount];
		}

		public void SetValue(int index,CSVValue value)
		{
			values [index] = value;
			indexForValue [value.StrValue] = index;
		}
	}
}
