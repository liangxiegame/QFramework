/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework 
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;

	public class EditorPathManager 
	{
		public const string DefaultPathConfigGenerateForder = "Assets/PTGameData/Path/Config";

		public const string DefaultPathScriptGenerateForder = "Assets/PTGameData/Path/Script";

		static Dictionary<string,PathConfig> mCachedPathConfigDict;

		static PathConfig Load(string configName) 
		{
			if (null == mCachedPathConfigDict || mCachedPathConfigDict.Count == 0) 
			{
				mCachedPathConfigDict = new Dictionary<string, PathConfig> ();
			}

			PathConfig retConfig = null;

			mCachedPathConfigDict.TryGetValue (configName, out retConfig);

			if (null == retConfig) 
			{
				retConfig = AssetDatabase.LoadAssetAtPath<PathConfig>(DefaultPathConfigGenerateForder + "/" + configName + ".asset");
				mCachedPathConfigDict.Add (configName, retConfig);
			}

			return retConfig;
		}

		public static PathConfig GetPathConfig<T>() 
		{
			string configName = typeof(T).ToString ();
			return Load (configName);
		}

		public static PathItem GetPathItem<T>(string pathName) 
		{
			string configName = typeof(T).ToString ();
			return Load (configName) [pathName];
		}

		public static string GetPath<T>(string pathName)  
		{
			return GetPathItem<T>(pathName).Path;
		}

		public static string GetAssetPath<T>(string pathName)
		{
			return "Assets/" + GetPathItem<T>(pathName).Path;
		}
	}
}
