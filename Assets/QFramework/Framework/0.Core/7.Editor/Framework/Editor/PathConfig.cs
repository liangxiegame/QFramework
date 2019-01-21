/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;
	using System.Collections.Generic;

	public enum PATH_ROOT
	{
		EditorPath,
		ApplicationDataPath,
		ApplicationPersistentDataPath,
		ApplicationStreamingAssetsPath,
		None
	}

	[System.Serializable]
	public class PathItem
	{
		[Header("描述")] [SerializeField] string m_Description = "";
		[SerializeField] PATH_ROOT m_Root = PATH_ROOT.ApplicationDataPath;
		[SerializeField] string m_Name = "";
		[SerializeField] string m_Path = "";
		[SerializeField] bool m_AutoCreateDirectory = false;

		public string Name
		{
			get { return m_Name; }
		}

		public string Path
		{
			get { return m_Path; }
		}

		/// <summary>
		/// Editor 时候用的
		/// </summary>
		/// <value>The property get code.</value>
		public string PropertyGetCode
		{
			get
			{
				if (string.IsNullOrEmpty(m_Name))
					return null;

				var retString = "m_" + m_Name;
				switch (m_Root)
				{
					case PATH_ROOT.EditorPath:
						retString = "\"" + "Assets/" + "\"" + " + " + retString;
						break;
					case PATH_ROOT.ApplicationDataPath:
						retString = "UnityEngine.Application.dataPath" + " + " + "\"/\"" + " + " + retString;
						break;
					case PATH_ROOT.ApplicationPersistentDataPath:
						retString = "UnityEngine.Application.persistentDataPath" + " + " + "\"/\"" + " + " + retString;
						break;
					case PATH_ROOT.ApplicationStreamingAssetsPath:
						retString = "UnityEngine.Application.streamingAssetsPath" + " + " + "\"/\"" + " + " + retString;
						break;
				}

				if (m_AutoCreateDirectory)
				{
					retString = "PTGame.Framework.Libs.IOUtils.CreateDirIfNotExists (" + retString + ")";
				}

				return retString;
			}
		}

		public string FullPath
		{
			get
			{
				switch (m_Root)
				{
					case PATH_ROOT.EditorPath:
						return "Assets/" + m_Path;
					case PATH_ROOT.ApplicationDataPath:
						return Application.dataPath + "/" + m_Path;
					case PATH_ROOT.ApplicationPersistentDataPath:
						return PATH_ROOT.ApplicationPersistentDataPath + "/" + m_Path;
					case PATH_ROOT.ApplicationStreamingAssetsPath:
						return PATH_ROOT.ApplicationStreamingAssetsPath + "/" + m_Path;
				}
				return m_Path;
			}
		}

		public string Description
		{
			get { return m_Description; }
		}

	}

	/// <summary>
	/// Path配置
	/// </summary>
	public class PathConfig : ScriptableObject
	{
		[Header("注意:每次修改该文件之后，一定要记得按Ctrl/Command + S")] [SerializeField] string m_Description;
		[SerializeField] List<PathItem> m_PathList;
		[Header("对应的脚本生成的路径")] [SerializeField] string m_ScriptGeneratePath;
		[Header("命名空间(默认QFramework)")] [SerializeField] string m_NameSpace;

		public List<PathItem> List
		{
			get { return m_PathList; }
		}

		Dictionary<string, PathItem> m_CachedPathDict;

		public string ScriptGeneratePath
		{
			get { return m_ScriptGeneratePath; }
		}

		public string Description
		{
			get { return m_Description; }
		}

		public string NameSpace
		{
			get { return m_NameSpace; }
		}

		/// <summary>
		/// 根据Path做索引
		/// </summary>
		public PathItem this[string pathName]
		{
			get
			{
				if (null == m_CachedPathDict)
				{
					m_CachedPathDict = new Dictionary<string, PathItem>();
					foreach (var pathItem in m_PathList)
					{
						if (!string.IsNullOrEmpty(pathItem.Name) && !m_CachedPathDict.ContainsKey(pathItem.Name))
						{
							m_CachedPathDict.Add(pathItem.Name, pathItem);
						}
						else
						{
							Debug.LogError(pathItem.Name + ":" + m_CachedPathDict.ContainsKey(pathItem.Name));
						}
					}
				}

				return m_CachedPathDict[pathName];
			}
		}
	}
}