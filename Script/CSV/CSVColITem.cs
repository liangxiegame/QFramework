using UnityEngine;
using System.Collections;

namespace QFramework {
	public class CSVColItem {
		public string AttribName {get; set;}
		public BASIC_TYPE AttribType;
		public int[] IntValue;
		public float[] FloatValue;
		public string[] StringValue;

		public CSVColItem(string name, int colLength, BASIC_TYPE attribType)
		{
			AttribName = name;
			AttribType = attribType;

			switch (AttribType) {
			case BASIC_TYPE.INT:
				IntValue = new int[colLength];
				break;
			case BASIC_TYPE.FLOAT:
				FloatValue = new float[colLength];
				break;
			case BASIC_TYPE.STRING:
				StringValue = new string[colLength];
				break;
			default:
				break;
			}
		}
	}
}
