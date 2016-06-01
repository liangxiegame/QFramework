using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {

	/// <summary>
	/// CSV数据管理器
	/// </summary>
	public class CSVMgr : QSingleton<CSVMgr> ,IMgr{

		// 
		private CSVMgr() {}
		public void Init()
		{

		}
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
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();
			TextAsset textAsset = Resources.Load(PATH + name, typeof(TextAsset)) as TextAsset;
			if (APP_CONFIG.DEBUG) {
				QPrint.Warn (textAsset.name);
			}
			mCachedTables.Add (textAsset.name, new CSVTable (textAsset.text));
			watch.Stop ();

			if (APP_CONFIG.DEBUG) {
				QPrint.Warn ("time:" + watch.ElapsedMilliseconds);
			}
			yield return 0;
		}

		public CSVValue GetValue(string tableName,string attribName,string headValue,string attrib)
		{
			var table = mCachedTables [tableName];
			int attribIndex = table.indexForAttribName [attribName];
			int rowIndex = table.colItems [attribIndex].indexForValue [headValue];
			var rowItem = table.rowItems [rowIndex];
			var retValue = rowItem.Values [rowItem.IndexForAttribName [attrib]];

			return retValue;
		}

		public CSVValue GetValue(string tableName,int index,string attrib)
		{
			var table = mCachedTables [tableName];
			var rowItem = table.rowItems [index];
			var retValue = rowItem.Values[rowItem.IndexForAttribName[attrib]];

			return retValue;
		}
	}
}
