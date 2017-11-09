/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * TODO: Copy To Persistant
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
	using System.Collections.Generic;

	/// <summary>
	/// 各种文件的读写复制操作,主要是对System.IO的一些封装
	/// </summary>
	public static class IOUtils 
	{
		/// <summary>
		/// 创建新的文件夹,如果存在则不创建
		/// </summary>
		public static string CreateDirIfNotExists(this string dirFullPath)
		{
			if (!Directory.Exists (dirFullPath)) 
			{
				Directory.CreateDirectory (dirFullPath);
			}
			return dirFullPath;
		}

		/// <summary>
		/// 删除文件夹，如果存在
		/// </summary>
		public static void DeleteDirIfExists(this string dirFullPath) 
		{
			if (Directory.Exists (dirFullPath)) 
			{
				Directory.Delete (dirFullPath, true);
			}
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
		
			
		public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true, string suffix = "")
		{
			List<string> pathList = new List<string>();
			DirectoryInfo di = new DirectoryInfo(dirABSPath);

			if (!di.Exists)
			{
				return pathList;
			}

			FileInfo[] files = di.GetFiles();
			foreach (FileInfo fi in files)
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
				DirectoryInfo[] dirs = di.GetDirectories();
				foreach (DirectoryInfo d in dirs)
				{
					pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
				}
			}

			return pathList;
		}

		public static List<string> GetDirSubDirNameList(this string dirABSPath)
		{
			List<string> nameList = new List<string>();
			DirectoryInfo di = new DirectoryInfo(dirABSPath);

			DirectoryInfo[] dirs = di.GetDirectories();
			foreach (DirectoryInfo d in dirs)
			{
				nameList.Add(d.Name);
			}

			return nameList;
		}

		public static string GetFileName(this string absOrAssetsPath)
		{
			string name = absOrAssetsPath.Replace("\\", "/");
			int lastIndex = name.LastIndexOf('/');

			if (lastIndex >= 0)
			{
				return name.Substring(lastIndex + 1);
			}
			else
			{
				return name;
			}
		}

		public static string GetFileNameWithoutExtend(this string absOrAssetsPath)
		{
			string fileName = GetFileName(absOrAssetsPath);
			int lastIndex = fileName.LastIndexOf('.');

			if (lastIndex >= 0)
			{
				return fileName.Substring(0, lastIndex);
			}
			else
			{
				return fileName;
			}
		}

		public static string GetFileExtendName(this string absOrAssetsPath)
		{
			int lastIndex = absOrAssetsPath.LastIndexOf('.');

			if (lastIndex >= 0)
			{
				return absOrAssetsPath.Substring(lastIndex);
			}

			return string.Empty;
		}

		public static string GetDirPath(this string absOrAssetsPath)
		{
			string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf('/');
			return name.Substring(0, lastIndex + 1);
		}


		public static string CombinePath(this string selfPath, string toCombinePath)
		{
			return Path.Combine(selfPath, toCombinePath);
		}
	}
}