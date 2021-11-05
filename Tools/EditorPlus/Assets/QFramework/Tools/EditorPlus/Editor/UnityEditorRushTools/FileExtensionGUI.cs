using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEditor.SearchService;

namespace QFramework.Editor
{
	[InitializeOnLoad]
	public class FileExtensionGUI
	{
		private static Vector2 offset = new Vector2(-25, 0);
		private static GUIStyle style;
		private static StringBuilder sb = new StringBuilder();
		private static string selectedGuid;

		private static HashSet<string> showExt = new HashSet<string>()
		{
			".tga",
			".tif",
			".psd",
			".png",
			".jpg",
			".raw",
			".fbx",
			".obj",
		};

		static FileExtensionGUI()
		{
			EditorApplication.projectWindowItemOnGUI += HandleOnGUI;
			Selection.selectionChanged += () =>
			{
				if (Selection.activeObject != null)
				{
					AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out selectedGuid,
						out long id);
				}
			};
		}

		private static bool ValidString(string str) => !string.IsNullOrEmpty(str) && str.Length > 7;

		private static void HandleOnGUI(string guid, Rect selectionRect)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			string extRaw = Path.GetExtension(path).ToLower();
			if (!showExt.Contains(extRaw)) { return; }

			bool selected = false;
			if (ValidString(guid) && ValidString(selectedGuid))
			{
				selected = string.Compare(guid, 0, selectedGuid, 0, 6) == 0;
			}

			sb.Clear().Append(extRaw);
			if (sb.Length > 0) { sb.Remove(0, 1); }

			string ext = sb.ToString();

			if (style == null) { style = new GUIStyle(EditorStyles.label); }

			style.normal.textColor = selected ? new Color32(255, 255, 255, 255) : new Color32(157, 157, 157, 255);
			Vector2 size = style.CalcSize(new GUIContent(ext));
			selectionRect.x = selectionRect.x + selectionRect.width - size.y - 16;
			Rect offsetRect = new Rect(selectionRect.position, selectionRect.size);
			EditorGUI.LabelField(offsetRect, ext, style);
		}
	}
}