using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace QFramework {
	/// <summary>
	/// 一张CSV表对应一个CSVTable
	/// </summary>
	public class CSVTable {

		public Type[] attribTypes;
		public Dictionary<string,int> attribs = new Dictionary<string,int>();

		public CSVRowItem[] rowItems;

		public Dictionary<string,CSVRowItem> itemsDict = new Dictionary<string,CSVRowItem>();
		public Dictionary<string,CSVColItem> attribColDict = new Dictionary<string, CSVColItem>();

		int rowCount;			// 行数
		int colCount;			// 列数

		int itemCount; 			// 项目数(行数 - 1)
		int attribCount;		// 每个项目的属性数

		public CSVTable(string csvText)
		{
			// 分行
			string[] rows = csvText.Split(CSVMgr.LINE_SPLIT_SYMBOL[0]);
			rowCount = rows.Length;
			itemCount = rows.Length - 1;

			//按‘|’进行拆分
			//创建二维数组
			string[][] items = new string[rows.Length][];
			//把csv中的数据储存在二位数组中
			for (int i = 0; i < rows.Length; i++)
			{
				items[i] = rows[i].Split(CSVMgr.SPLIT_SYMBOL[0]);
			}

			// 列数
			colCount = items [0].Length;
			attribCount = items [0].Length;

			attribTypes = new Type[colCount]; // head

			// 处理第一行
			for (int i = 0; i < items [0].Length; i++) {

				string[] attrib_type = items [0] [i].Split (":"[0]);

				attribs[attrib_type [0]] = i;	// 属性 
				attribTypes[i] = TypeForString(attrib_type [1]);	// 类型
			}

			// 开始读取数据
			rowItems = new CSVRowItem[itemCount];

			// 
			for (int rowIndex = 1; rowIndex < rowCount; rowIndex++) {
				rowItems [rowIndex - 1] =  new CSVRowItem(items [rowIndex],attribTypes);

				itemsDict [rowItems [rowIndex - 1].Name] = rowItems [rowIndex - 1];
			}
		}

		Type TypeForString(string typeName)
		{
			switch (typeName) {
			case "string":
				return "".GetType();
			case "int":
				return 1.GetType();
			case "flaot":
				return 1.0f.GetType();
			default:
				return "".GetType();
			}
		}
	}
}