/****************************************************************************
 * Copyright (c) 2015 ~ 2024 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEditor;
using UnityEngine;

namespace QFramework
{
	public static class AssetBundleExporter
	{
		public static void BuildDataTable(string[] abNames = null,string outputPath = null,bool appendHash = false)
		{
			Debug.Log("Start Default BuildAssetDataTable!");
			var table = new ResDatas();
			if (appendHash)
			{
				ConfigFileUtility.AddABInfo2ResDatasAppendHash(table, abNames);
			}
			else
			{
				ConfigFileUtility.AddABInfo2ResDatas(table, abNames);

			}
			var filePath =
				(outputPath ?? (AssetBundlePathHelper.StreamingAssetsPath + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER)).CreateDirIfNotExists() +
				ResDatas.FileName;
			
			table.Save(filePath);
			AssetDatabase.Refresh();
		}
	}
}
