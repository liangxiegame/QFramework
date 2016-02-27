using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 一个项目的属性
/// </summary>
public class CSVRowItem {

	public Dictionary<string,int> IndexForAttribName;
	public CSVValue[] Values;

	public CSVRowItem(CSVValue[] values,Dictionary<string,int> indexForAttribName)
	{
		IndexForAttribName = indexForAttribName;
		Values = values;
//		for (int i = 0;i <values.Length;i++) {
//			QPrint.Warn (values [i]);
//
//		}
	}

}