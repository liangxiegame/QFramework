using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {

	public enum BASIC_VALUE{
		INT = 1,
		FLOAT = 1 << 1,
		STRING = 1 << 2,
	}

	/// <summary>
	/// 一张CSV表对应一个CSVTable
	/// </summary>
	public class CSVTable {
		
		public BASIC_VALUE[] attribTypes;
		public Dictionary<string,int> attribs = new Dictionary<string,int>();

		public CSVItem[] itemsArray;
		public Dictionary<string,CSVItem> itemsDict = new Dictionary<string,CSVItem>();

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

			attribTypes = new BASIC_VALUE[colCount];

			// 处理第一行
			for (int i = 0; i < items [0].Length; i++) {
				
				string[] attrib_type = items [0] [i].Split (":"[0]);

				attribs[attrib_type [0]] = i;	// 属性 
				attribTypes[i] = TypeForString(attrib_type [1]);	// 类型
			}

			// 开始读取数据
			itemsArray = new CSVItem[itemCount];

			// 
			for (int rowIndex = 1; rowIndex < rowCount; rowIndex++) {
				itemsArray [rowIndex - 1] =  new CSVItem(items [rowIndex]);

				itemsDict [itemsArray [rowIndex - 1].Name] = itemsArray [rowIndex - 1];
			}
		}

		BASIC_VALUE TypeForString(string typeName)
		{
			switch (typeName) {
			case "string":
				return BASIC_VALUE.STRING;
			case "int":
				return BASIC_VALUE.INT;
			case "flaot":
				return BASIC_VALUE.FLOAT;
			default:
				return BASIC_VALUE.STRING;
			}
		}
	}

	/// <summary>
	/// 一个项目的属性
	/// </summary>
	public class CSVItem {
		public string Name { get; set;}
		public int Index { get; set; }
		public Dictionary<string,object> attribs = new Dictionary<string,object> ();
		public string[] Values;

		public CSVItem(string[] colsText)
		{
			Values = new string[colsText.Length];

			for (int i = 0 ; i  < Values.Length;i++)
			{
				Values [i] = colsText [i];
			}

			Index = int.Parse(Values [0]);
			Name =  Values [1];
			QPrint.Warn ("index:" + Index + " name:" + Name);
		}
	}

	/// <summary>
	/// CSV数据管理器
	/// </summary>
	public class CSVMgr : QSingleton<CSVMgr> {

		// 
		private CSVMgr() {}

		// 
		public const string PATH = "CSV/";
		public const string LINE_SPLIT_SYMBOL = "\r";
		public const string SPLIT_SYMBOL = "\t";
	
		// 表数据
		Dictionary<string,CSVTable> mCachedTables = new Dictionary<string,CSVTable>();

		/// <summary>
		/// 读取数据
		/// </summary>
		public IEnumerator LoadData()
		{
			App.Instance ().StartCoroutine (LoadTable ("prop"));

			yield return 0;
		}
			
		/// <summary>
		/// 读一张表
		/// </summary>
		IEnumerator LoadTable(string name)
		{
			
			TextAsset binAsset = Resources.Load(PATH + name, typeof(TextAsset)) as TextAsset;
			QPrint.Warn (binAsset.name);
			mCachedTables.Add (binAsset.name, new CSVTable (binAsset.text));
			yield return new WaitForEndOfFrame ();
		}

		// 获取数据
		public string GetData(string tableName,string propName,string AttribName)
		{
			var table = mCachedTables [tableName];
			var item = table.itemsDict [propName];

			return item.Values[table.attribs[AttribName]];
		}
	}
}
