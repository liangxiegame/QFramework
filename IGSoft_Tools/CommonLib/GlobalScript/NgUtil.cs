// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public	class NgUtil
{
	// Log ------------------------------------------------------------------
	public static void LogDevelop(object msg)
	{
//  		Debug.Log(msg);
	}

	public static void LogMessage(object msg)
	{
// 		Debug.Log("-----------------------------------------------------");
		Debug.Log(msg);
	}

	public static void LogError(object msg)
	{
		Debug.Log(msg);
	}

	// Math ------------------------------------------------------------------
	// 높이와 길이로 각도 구하기
	public static float GetArcRadian(float fHeight, float fWidth)
	{
		float	fHalfWidth	= fWidth/2.0f;
		float	r = GetArcRadius(fHeight, fWidth);
		float	a = Mathf.Sin(fHalfWidth / r);
		return a * 2;
	}

	// 높이와 길이로 반지름 구하기
	public static float GetArcRadius(float fHeight, float fWidth)
	{
		float	fHalfWidth	= fWidth/2.0f;
		float	r = (fHeight*fHeight + fHalfWidth*fHalfWidth) / (2 * fHeight);
		return r;
	}

	// 높이와 길이로 호 구하기
	public static float GetArcLength(float fHeight, float fWidth)
	{
		float	fHalfWidth	= fWidth/2.0f;
		float	r = GetArcRadius(fHeight, fWidth);
		float	a = fHalfWidth / r;
		float	z = r * (2 * a);
		return z;
	}

	public static int NextPowerOf2(int val)
	{
		int newVal = Mathf.ClosestPowerOfTwo(val);

		while (newVal < val)
			newVal <<= 1;
		return newVal;
	}

	// Clear ------------------------------------------------------------------
	public static void ClearStrings(string[] strings)
	{
		if (strings == null)
			return;
		for (int n = 0; n < strings.Length; n++)
			strings[n] = "";
	}

	public static void ClearBools(bool[] bools)
	{
		if (bools == null)
			return;
		for (int n = 0; n < bools.Length; n++)
			bools[n] = false;
	}

	public static void ClearObjects(Object[] objects)
	{
		if (objects == null)
			return;
		for (int n = 0; n < objects.Length; n++)
			objects[n] = null;
	}

// 	// List ------------------------------------------------------------------
// 	public static bool ListContains(List<Texture2D> list, Texture2D obj)
// 	{
// 		for (int n = 0; n < list.Count; n++)
// 			if (list[n] == obj)
// 				return true;
// 		return false;
// 	}
// 
// 	public static int ListIndex(List<Texture2D> list, Texture2D obj)
// 	{
// 		for (int n = 0; n < list.Count; n++)
// 			if (list[n] == obj)
// 				return n;
// 		return -1;
// 	}
}
