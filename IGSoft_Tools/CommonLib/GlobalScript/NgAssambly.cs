// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Reflection; 

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NgAssembly
{
	// Attribute ------------------------------------------------------------------------
	public static BindingFlags	m_bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;	// | BindingFlags.DeclaredOnly;

	// Field / Property ------------------------------------------------------------------------
	public static T GetReference<T>(object inObj, string fieldName) where T : class
	{
		return GetField(inObj, fieldName) as T;
	}

	public static T GetValue<T>(object inObj, string fieldName) where T : struct
	{
		return (T)GetField(inObj, fieldName);
	}

	public static void SetField(object inObj, string fieldName, object newValue)
	{
		FieldInfo info = inObj.GetType().GetField(fieldName);
		if (info != null)
			info.SetValue(inObj, newValue);
	}

	private static object GetField(object inObj, string fieldName)
	{
		object ret = null;
		FieldInfo info = inObj.GetType().GetField(fieldName);
		if (info != null)
			ret = info.GetValue(inObj);
		return ret;
	}

	public static void SetProperty(object srcObj, string fieldName, object newValue)
	{
		PropertyInfo info = srcObj.GetType().GetProperty(fieldName, m_bindingAttr);
		if (info != null && info.CanWrite)
			info.SetValue(srcObj, newValue, null);
		else Debug.LogWarning(fieldName + " could not be write.");
	}

	public static object GetProperty(object srcObj, string fieldName)
	{
		object ret = null;
		PropertyInfo info = srcObj.GetType().GetProperty(fieldName, m_bindingAttr);
		if (info != null && info.CanRead && info.GetIndexParameters().Length == 0)
		{
			ret = info.GetValue(srcObj, null);
		} else Debug.LogWarning(fieldName + " could not be read.");
		return ret;
	}

	public static void LogFieldsPropertis(Object srcObj)
	{
		if (srcObj == null)
			return;

		string		log = "=====================================================================\r\n";

		FieldInfo[] fieldinfos = srcObj.GetType().GetFields(m_bindingAttr);
		foreach (FieldInfo fieldinfo in fieldinfos)
			log += string.Format("{0}   {1,-30}\r\n", fieldinfo.Name, fieldinfo.GetValue(srcObj).ToString());

		Debug.Log(log);
		log = "";

		PropertyInfo[] proinfos = srcObj.GetType().GetProperties(m_bindingAttr);
		foreach (PropertyInfo proinfo in proinfos)
			if (proinfo.CanRead && proinfo.GetIndexParameters().Length == 0)
				log += string.Format("{0,-10}{1,-30}   {2,-30}\r\n", proinfo.CanWrite, proinfo.Name, proinfo.GetValue(srcObj, null).ToString());

		log = log + "=====================================================================\r\n";
		Debug.Log(log);
	}

	// Gizmos --------------------------------------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
	public static bool AreGizmosVisible()
	{
		Assembly		asm		= Assembly.GetAssembly(typeof(UnityEditor.Editor));
		System.Type		type	= asm.GetType("UnityEditor.GameView");

		if (type != null)
		{
			EditorWindow	window		= UnityEditor.EditorWindow.GetWindow(type);
			FieldInfo		gizmosField = type.GetField("m_Gizmos", BindingFlags.NonPublic | BindingFlags.Instance);
			if (gizmosField != null)
				return (bool)gizmosField.GetValue(window);
		}
		return false;
	}

	public static bool SetGizmosVisible(bool bVisible)
	{
		System.Reflection.Assembly	asm		= System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
		System.Type					type	= asm.GetType("UnityEditor.GameView");

		if (type != null)
		{
			UnityEditor.EditorWindow	window = UnityEditor.EditorWindow.GetWindow(type);
			FieldInfo					gizmosField = type.GetField("m_Gizmos", BindingFlags.NonPublic | BindingFlags.Instance);
			if (gizmosField != null)
				gizmosField.SetValue(window, bVisible);
		}
		return false;
	}
#endif
}
