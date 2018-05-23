/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;	
	using UnityEngine;

	/// <summary>
	/// 各种文件的读写复制操作,主要是对System.IO的一些封装
	/// </summary>
	public static class IOExtension
	{
		public static void Example()
		{
			var testDir = Application.persistentDataPath.CombinePath("TestFolder");
			testDir.CreateDirIfNotExists();

			Debug.Log(Directory.Exists(testDir));
			testDir.DeleteDirIfExists();
			Debug.Log(Directory.Exists(testDir));

			var testFile = testDir.CombinePath("test.txt");
			testDir.CreateDirIfNotExists();
			File.Create(testFile);
			testFile.DeleteFileIfExists();
		}
		
		/// <summary>
		/// 创建新的文件夹,如果存在则不创建
		/// </summary>
		public static string CreateDirIfNotExists(this string dirFullPath)
		{
			if (!Directory.Exists(dirFullPath))
			{
				Directory.CreateDirectory(dirFullPath);
			}

			return dirFullPath;
		}

		/// <summary>
		/// 删除文件夹，如果存在
		/// </summary>
		public static void DeleteDirIfExists(this string dirFullPath)
		{
			if (Directory.Exists(dirFullPath))
			{
				Directory.Delete(dirFullPath, true);
			}
		}

		/// <summary>
		/// 清空 Dir,如果存在。
		/// </summary>
		public static void EmptyDirIfExists(this string dirFullPath)
		{
			if (Directory.Exists(dirFullPath))
			{
				Directory.Delete(dirFullPath, true);
			}
 
			Directory.CreateDirectory(dirFullPath);
		}
		
		/// <summary>
		/// 删除文件 如果存在
		/// </summary>
		/// <param name="fileFullPath"></param>
		/// <returns> True if exists</returns>
		public static bool DeleteFileIfExists(this string fileFullPath)
		{
			if (File.Exists(fileFullPath))
			{
				File.Delete(fileFullPath);
				return true;
			}

			return false;
		}

		public static string CombinePath(this string selfPath, string toCombinePath)
		{
			return Path.Combine(selfPath, toCombinePath);
		}

		#region 未经过测试

		/// <summary>
		/// 保存文本
		/// </summary>
		/// <param name="text"></param>
		/// <param name="path"></param>
		public static void SaveText(this string text, string path)
		{
			path.DeleteFileIfExists();

			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (var sr = new StreamWriter(fs))
				{
					sr.Write(text); //开始写入值
				}
			}
		}


		/// <summary>
		/// 读取文本
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static string ReadText(this FileInfo file)
		{
			return ReadText(file.FullName);
		}

		/// <summary>
		/// 读取文本
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static string ReadText(this string fileFullPath)
		{
			var result = string.Empty;

			using (var fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
			{
				using (var sr = new StreamReader(fs))
				{
					result = sr.ReadToEnd();
				}
			}

			return result;
		}

#if UNITY_EDITOR
		/// <summary>
		/// 打开文件夹
		/// </summary>
		/// <param name="path"></param>
		public static void OpenFolder(string path)
		{
#if UNITY_STANDALONE_OSX
			System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
			System.Diagnostics.Process.Start("explorer.exe", path);
#endif
		}
#endif

		/// <summary>
		/// 获取文件夹名
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetDirectoryName(string fileName)
		{
			fileName = IOExtension.MakePathStandard(fileName);
			return fileName.Substring(0, fileName.LastIndexOf('/'));
		}

		/// <summary>
		/// 获取文件名
		/// </summary>
		/// <param name="path"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static string GetFileName(string path, char separator = '/')
		{
			path = IOExtension.MakePathStandard(path);
			return path.Substring(path.LastIndexOf(separator) + 1);
		}

		/// <summary>
		/// 获取不带后缀的文件名
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static string GetFileNameWithoutExtention(string fileName, char separator = '/')
		{
			return GetFilePathWithoutExtention(GetFileName(fileName, separator));
		}

		/// <summary>
		/// 获取不带后缀的文件路径
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetFilePathWithoutExtention(string fileName)
		{
			if (fileName.Contains("."))
				return fileName.Substring(0, fileName.LastIndexOf('.'));
			return fileName;
		}

		/// <summary>
		/// 获取streamingAssetsPath
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetStreamPath(string fileName)
		{
			string str = Application.streamingAssetsPath + "/" + fileName;
			if (Application.platform != RuntimePlatform.Android)
			{
				str = "file://" + str;
			}

			return str;
		}

		/// <summary>
		/// 工程根目录
		/// </summary>
		public static string projectPath
		{
			get
			{
				DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
				return MakePathStandard(directory.Parent.FullName);
			}
		}

		/// <summary>
		/// 使目录存在,Path可以是目录名必须是文件名
		/// </summary>
		/// <param name="path"></param>
		public static void MakeFileDirectoryExist(string path)
		{
			string root = Path.GetDirectoryName(path);
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}
		}

		/// <summary>
		/// 使目录存在
		/// </summary>
		/// <param name="path"></param>
		public static void MakeDirectoryExist(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// 结合目录
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static string Combine(params string[] paths)
		{
			string result = "";
			foreach (string path in paths)
			{
				result = Path.Combine(result, path);
			}

			result = MakePathStandard(result);
			return result;
		}

		/// <summary>
		/// 获取父文件夹
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetPathParentFolder(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			return Path.GetDirectoryName(path);
		}

		/// <summary>
		/// 将绝对路径转换为相对于Asset的路径
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ConvertAbstractToAssetPath(string path)
		{
			path = MakePathStandard(path);
			return MakePathStandard(path.Replace(projectPath + "/", ""));
		}

		/// <summary>
		/// 将绝对路径转换为相对于Asset的路径且去除后缀
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ConvertAbstractToAssetPathWithoutExtention(string path)
		{
			return IOExtension.GetFilePathWithoutExtention(ConvertAbstractToAssetPath(path));
		}

		/// <summary>
		/// 将相对于Asset的路径转换为绝对路径
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ConvertAssetPathToAbstractPath(string path)
		{
			path = MakePathStandard(path);
			return Combine(projectPath, path);
		}

		/// <summary>
		/// 将相对于Asset的路径转换为绝对路径且去除后缀
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ConvertAssetPathToAbstractPathWithoutExtention(string path)
		{
			return IOExtension.GetFilePathWithoutExtention(ConvertAssetPathToAbstractPath(path));
		}

		/// <summary>
		/// 使路径标准化，去除空格并将所有'\'转换为'/'
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string MakePathStandard(string path)
		{
			return path.Trim().Replace("\\", "/");
		}

		/// <summary>
		/// 去除‘..’用路径替换
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string Normalize(string path)
		{
			var normalized = path;
			normalized = Regex.Replace(normalized, @"/\./", "/");
			if (normalized.Contains(".."))
			{
				var list = new List<string>();
				var paths = normalized.Split('/');
				foreach (var name in paths)
				{
					// 首位是".."无法处理的
					if (name.Equals("..") && list.Count > 0)
						list.RemoveAt(list.Count - 1);
					else
						list.Add(name);
				}

				normalized = list.Join("/");
			}

			if (path.Contains("\\"))
			{
				normalized = normalized.Replace("\\", "/");
			}

			return normalized;
		}

		public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true, string suffix = "")
		{
			var pathList = new List<string>();
			var di = new DirectoryInfo(dirABSPath);

			if (!di.Exists)
			{
				return pathList;
			}

			var files = di.GetFiles();
			foreach (var fi in files)
			{
				if (!string.IsNullOrEmpty(suffix))
				{
					if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
					{
						continue;
					}
				}

				pathList.Add(fi.FullName);
			}

			if (isRecursive)
			{
				var dirs = di.GetDirectories();
				foreach (var d in dirs)
				{
					pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
				}
			}

			return pathList;
		}

		public static List<string> GetDirSubDirNameList(this string dirABSPath)
		{
			var di = new DirectoryInfo(dirABSPath);

			var dirs = di.GetDirectories();

			return dirs.Select(d => d.Name).ToList();
		}

		public static string GetFileName(this string absOrAssetsPath)
		{
			var name = absOrAssetsPath.Replace("\\", "/");
			var lastIndex = name.LastIndexOf("/");

			return lastIndex >= 0 ? name.Substring(lastIndex + 1) : name;
		}

		public static string GetFileNameWithoutExtend(this string absOrAssetsPath)
		{
			var fileName = GetFileName(absOrAssetsPath);
			var lastIndex = fileName.LastIndexOf(".");

			return lastIndex >= 0 ? fileName.Substring(0, lastIndex) : fileName;
		}

		public static string GetFileExtendName(this string absOrAssetsPath)
		{
			var lastIndex = absOrAssetsPath.LastIndexOf(".");

			if (lastIndex >= 0)
			{
				return absOrAssetsPath.Substring(lastIndex);
			}

			return string.Empty;
		}

		public static string GetDirPath(this string absOrAssetsPath)
		{
			var name = absOrAssetsPath.Replace("\\", "/");
			var lastIndex = name.LastIndexOf("/");
			return name.Substring(0, lastIndex + 1);
		}
		
		public static string GetLastDirName(this string absOrAssetsPath)
		{
			var name = absOrAssetsPath.Replace("\\", "/");
			var dirs = name.Split('/');

			return dirs[dirs.Length - 2];
		}

		#endregion
	}
}