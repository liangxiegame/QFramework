using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {

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
		/// 读一张表
		/// </summary>
		public IEnumerator LoadTable(string name)
		{
			
			TextAsset binAsset = Resources.Load(PATH + name, typeof(TextAsset)) as TextAsset;
			QPrint.Warn (binAsset.name);
			mCachedTables.Add (binAsset.name, new CSVTable (binAsset.text));
			yield return new WaitForEndOfFrame ();
		}

		// 获取数据
		public string GetData(string tableName,string itemName,string attribName)
		{
			var table = mCachedTables [tableName];
			var item = table.itemsDict [itemName];

			return item.Values[table.attribs[attribName]];
		}

		public string GetIntData(string tableName,string itemName,string attribName)
		{
			var table = mCachedTables [tableName];
			var item = table.itemsDict [itemName];
			return item.Values[table.attribs[attribName]];
		}

		public string GetIntData(string tableName,int id,string attribName)
		{
			return null;
		}

		public CSVTable GetTable(string tableName)
		{
			return null;
		}

		public CSVRowItem GetItem(string itemName)
		{
			return null;
		}


	}
}
