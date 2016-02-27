using UnityEngine;
using System.Collections;

public class CSVValue  {
	
	public int RowIndex{ get; set; }
	public int ColIndex{ get; set; }

	public int IntValue{ get; set;}
	public float FloatValue{ get; set; }
	public string StrValue{ get; set; }

	public string Head;

	BASIC_TYPE mType;

	public CSVValue(string head,string strValue,BASIC_TYPE type,int rowIndex,int colIndex)
	{
		Head = head;
		StrValue = strValue;
		mType = type;
		RowIndex = rowIndex;
		ColIndex = colIndex;

		// 解析
		switch (mType) {
		case BASIC_TYPE.INT:
			IntValue = int.Parse (StrValue);
			break;
		case BASIC_TYPE.FLOAT:
			FloatValue = float.Parse (StrValue);
			break;
		default:
			break;
		}
	}
		
	public override string ToString()
	{
		return "rowIndex:" + RowIndex + " ColIndex:" + ColIndex + " IntValue:" +  IntValue + " FloatValue:" + FloatValue + " StrValue:" + StrValue + " Head:" + Head + mType;
	}
}
