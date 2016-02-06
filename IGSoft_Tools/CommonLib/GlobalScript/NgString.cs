// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public	class NgConvert
{
	// string ----------------------------------------------------
	public static string GetTabSpace(int nTab)
	{
		string sp = "    ";
		string re = "";
		for (int n = 0; n < nTab; n++)
			re += sp;
		return re;
	}

	public static string[] GetIntStrings(int start, int count)
	{
		string[] ret = new string[count];
		for (int n = start; n < count; n++)
			ret[n] = n.ToString();
		return ret;
	}

	public static int[] GetIntegers(int start, int count)
	{
		int[] ret = new int[count];
		for (int n = start; n < count; n++)
			ret[n] = n;
		return ret;
	}

	// array ----------------------------------------------------
	public static ArrayList ToArrayList<TT>(TT[] data)
	{
		ArrayList arlist = new ArrayList();

		for (int i = 0; i < data.Length; i++)
			arlist.Add(data[i]);
		return arlist;
	}

	public static TT[] ToArray<TT>(ArrayList data)
	{
		TT[] arr = new TT[data.Count];

		int count = 0;

		foreach (TT data1 in data)
		{
			if (data1 != null)
				arr[count] = data1;
			count++;
		}
		return arr;
	}

	public static TT[] ResizeArray<TT>(TT[] src, int nResize)
	{
		TT[] dst = new TT[nResize];
		for (int n = 0; n < src.Length && n < nResize; n++)
			dst[n] = src[n];
		return dst;
	}

	public static TT[] ResizeArray<TT>(TT[] src, int nResize, TT defaultValue)
	{
		TT[] dst = new TT[nResize];
		int n;
		for (n = 0; n < src.Length && n < nResize; n++)
			dst[n] = src[n];
		for (; n < dst.Length; n++)
			dst[n] = defaultValue;
		return dst;
	}

	public static string[] ResizeArray(string[] src, int nResize)
	{
		string[] dst = new string[nResize];
		for (int n = 0; n < src.Length && n < nResize; n++)
			dst[n] = src[n];
		return dst;
	}

	public static GameObject[] ResizeArray(GameObject[] src, int nResize)
	{
		GameObject[] dst = new GameObject[nResize];
		for (int n = 0; n < src.Length && n < nResize; n++)
			dst[n] = src[n];
		return dst;
	}

	public static GUIContent[] ResizeArray(GUIContent[] src, int nResize)
	{
		GUIContent[] dst = new GUIContent[nResize];
		for (int n = 0; n < src.Length && n < nResize; n++)
			dst[n] = src[n];
		return dst;
	}

	public static GUIContent[] StringsToContents(string[] strings)
	{
		if (strings == null)
			return null;

		GUIContent[]	cons = new GUIContent[strings.Length];
		for (int n = 0; n < strings.Length; n++)
			cons[n] = new GUIContent(strings[n], strings[n]);
		return cons;
	}

	public static string[] ContentsToStrings(GUIContent[] contents)
	{
		if (contents == null)
			return null;

		string[]	cons = new string[contents.Length];
		for (int n = 0; n < contents.Length; n++)
			cons[n] = contents[n].text;
		return cons;
	}

	// convert ----------------------------------------------------
	public static uint ToUint(string value, uint nDefaultValue)
	{
		uint result;
		value = value.Trim();
		if (value == "")
			value = "0";
		if (uint.TryParse(value, out result))
			return result;
		return nDefaultValue;
	}

	public static int ToInt(string value, int nDefaultValue)
	{
		int result;
		value = value.Trim();
		if (value == "")
			value = "0";
		if (int.TryParse(value, out result))
			return result;
		return nDefaultValue;
	}

	public static float ToFloat(string value, float fDefaultValue)
	{
		float result;
		value = value.Trim();
		if (value == "")
			value = "0";
		if (float.TryParse(value, out result))
			return result;
		return fDefaultValue;
	}

	public static string GetVaildFloatString(string strInput, ref float fCompleteValue)
	{
		float	fResult;
		int		nIndex = 0;
		int		nComma = 0;
		string	intStr = "0123456789"; 
		strInput = strInput.Trim();

		while (nIndex < strInput.Length)
		{
			if (intStr.Contains(strInput[nIndex].ToString()))
			{
				nIndex++;
				continue;
			}
			if (strInput[nIndex] == '+' || strInput[nIndex] == '-')
			{
				if (nIndex == 0)
				{
					nIndex++;
					continue;
				}
				strInput = strInput.Remove(nIndex, 1);
				continue;
			}
			if (strInput[nIndex] == '.')
			{
				nComma++;
				nIndex++;
				if (nComma == 1)
					continue;
				strInput = strInput.Remove(nIndex-1, 1);
				continue;
			}
			nIndex++;
		}

		if (strInput == "" || float.TryParse(strInput, out fResult) == false)
			return strInput;
		if (strInput[strInput.Length-1] == '.')
			return strInput;

		fCompleteValue	= fResult;
		return null;
	}
}

