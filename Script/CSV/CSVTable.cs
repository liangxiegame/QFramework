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
		
		int rowCount;			// 行数
		int colCount;			// 列数

		int itemCount; 			// 项目数(行数 - 1)
		int attribCount;		// 每个项目的属性数

		//类型
		public BASIC_TYPE[] types;
		// 表头
		public string[] heads;	

		// 存储字符串缓存的2维数组,读取完后删掉 
		public string[][] strs;
		public CSVValue[][] values;
		public CSVRowItem[] rowItems;
		public CSVColItem[] colItems;

		public Dictionary<string,int> indexForAttribName = new Dictionary<string,int>();

		public CSVTable(string csvText)
		{
			CreateStrs (csvText);		// 创建字符串二维数组
			CreateHeads ();				// 创建表头
			CreateValues();				// 创建元素
			CreateRowItems();			// 创建横项
			CreateColItems();			// 创建列项
			CreateRelation();			// 创建关联
		}

		// 创建2维数组字符串
		private void CreateStrs(string csvText)
		{
			// 分行
			string[] rows = csvText.Split (CSVMgr.LINE_SPLIT_SYMBOL [0]);
			rowCount = rows.Length;
			itemCount = rows.Length - 1;

			// 创建二维字符串数组
			strs = new string[rowCount][];
			for (int i = 0; i < rowCount; i++) {
				strs [i] = rows [i].Split (CSVMgr.SPLIT_SYMBOL [0]);
			}

		}

		// 创建表头
		private void CreateHeads()
		{
			// 列数
			colCount = strs[0].Length;
			attribCount = strs [0].Length;

			types = new BASIC_TYPE[colCount];
			heads = new string[colCount];
			// 处理第一行
			for (int i = 0; i < strs [0].Length; i++) {
				string[] attrib_type = strs [0] [i].Split (":" [0]);
				heads [i] = attrib_type [0];
				types [i] = TypeForString (attrib_type [1]);
			}
		}

		// 创建值
		private void CreateValues()
		{
			values = new CSVValue[itemCount][];

			for (int rowIndex = 0; rowIndex < itemCount; rowIndex++) {
				values[rowIndex] = new CSVValue[attribCount];

				for (int colIndex = 0; colIndex < attribCount; colIndex++) {
					values [rowIndex] [colIndex] = new CSVValue (heads[colIndex],strs[rowIndex + 1][colIndex],types[colIndex],rowIndex,colIndex);
				}
			}
		}


		private void CreateRowItems()
		{
			rowItems = new CSVRowItem[itemCount];
			for (int rowIndex = 0; rowIndex < itemCount; rowIndex++) {
				rowItems [rowIndex] = new CSVRowItem (values [rowIndex],indexForAttribName);
			}
		}

		private void CreateColItems()
		{
			colItems = new CSVColItem[attribCount];
			for (int colIndex = 0; colIndex < attribCount; colIndex++) {
				colItems [colIndex] = new CSVColItem (heads [colIndex], types [colIndex], itemCount);

				for (int itemIndex = 0; itemIndex < itemCount; itemIndex++) {
					colItems [colIndex].SetValue (itemIndex, values [itemIndex] [colIndex]);
				}
			}

		}

		private void CreateRelation()
		{
			for (int attribIndex = 0; attribIndex < attribCount; attribIndex++) {
				indexForAttribName [heads [attribIndex]] = attribIndex;
			}
		}

		// 内部使用
		private BASIC_TYPE TypeForString(string name)
		{
			if (name.CompareTo ("int") == 0) {
				return BASIC_TYPE.INT;
			} else if (name.CompareTo ("float") == 0) {
				return BASIC_TYPE.FLOAT;
			} else if (name.CompareTo ("string") == 0) {
				return BASIC_TYPE.STRING;
			}
				
			return BASIC_TYPE.STRING;
		}
	}

}