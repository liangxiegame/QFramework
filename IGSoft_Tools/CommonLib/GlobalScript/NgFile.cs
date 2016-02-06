// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.IO;

public	class NgFile
{
	// file ----------------------------------------------------
	public static string PathSeparatorNormalize(string path)
	{
		char[] bufStr = path.ToCharArray();

		for (int n = 0; n < path.Length; n++)
		{
			if (path[n] == '/' || path[n] == '\\')
				bufStr[n] = '/';
		}
		path = new string(bufStr);
		return path;
	}

	public static string CombinePath(string path1, string path2)
	{
		return PathSeparatorNormalize(Path.Combine(path1, path2));
	}

	public static string CombinePath(string path1, string path2, string path3)
	{
		return PathSeparatorNormalize(Path.Combine(Path.Combine(path1, path2), path3));
	}

	public static string GetSplit(string path, int nIndex)
	{
		if (nIndex < 0)
			return path;
		string[]	strs = path.Split('/', '\\');
		if (nIndex < strs.Length)
			return strs[nIndex];
		return "";
	}

	public static string GetFilename(string path)
	{
		for (int n = path.Length-1; 0 <= n; n--)
			if (path[n] == '/' || path[n] == '\\')
			{
				if (n == path.Length-1)
					return "";
				return TrimFileExt(path.Substring(n+1));
			}
		return TrimFileExt(path);
	}

	public static string GetFilenameExt(string path)
	{
		for (int n = path.Length-1; 0 <= n; n--)
			if (path[n] == '/' || path[n] == '\\')
			{
				if (n == path.Length-1)
					return "";
				return path.Substring(n+1);
			}
		return path;
	}

	public static string GetFileExt(string path)
	{
		for (int n = path.Length-1; 0 <= n; n--)
			if (path[n] == '.')
				return path.Substring(n+1);
		return "";
	}

	public static string TrimFilenameExt(string path)
	{
		for (int n = path.Length-1; 0 <= n; n--)
			if (path[n] == '/' || path[n] == '\\')
				return path.Substring(0, n);
		return "";
	}

	public static string TrimFileExt(string filename)
	{
		for (int n = filename.Length-1; 0 <= n; n--)
			if (filename[n] == '.')
				return filename.Substring(0, n);
		return filename;
	}

	public static string TrimLastFolder(string path)
	{
		for (int n = path.Length-1; 0 <= n; n--)
		{
			if (path[n] == '/' || path[n] == '\\')
				if (n != path.Length-1)
					return path.Substring(0, n);
		}
		return "";
	}

	public static string GetLastFolder(string path)
	{
		for (int n = path.Length-1; 0 <= n; n--)
		{
			if (path[n] == '/' || path[n] == '\\')
				if (n != path.Length-1)
				{
					if (path[path.Length-1] == '/' || path[path.Length-1] == '\\')
						 return path.Substring(n+1, path.Length-n-2);
					else return path.Substring(n+1, path.Length-n-1);
				}
		}
		return path;
	}

	// File Compare ------------------------------------------------------------------
	public static bool CompareExtName(string srcPath, string tarLowerExt, bool bCheckCase)
	{
		if (bCheckCase)
		{
			return (GetFilenameExt(srcPath).ToLower() == tarLowerExt);
		} else {
			return (GetFilenameExt(srcPath) == tarLowerExt);
		}
	}
}
