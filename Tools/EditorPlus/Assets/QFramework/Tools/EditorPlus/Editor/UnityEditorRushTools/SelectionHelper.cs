// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 09:57
//  *
//  * 取消了对创建Image和Text的支持，请使用Preset功能
//  * Tips：新版本中使用Preset为一些组件做一个默认值直接创建
//  *
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace QFramework.Editor
{
	internal static class SelectionHelper
	{
		[InitializeOnLoadMethod]
		private static void Start()
		{
			//在Hierarchy面板按鼠标中键相当于开关GameObject
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;

			//在Project面板按鼠标中键相当于Show In Explorer
			EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGui;

			//在Scene窗口按空格键相当于开关GameObject
#if UNITY_5_6 || UNITY_2017 || UNITY_2018
			SceneView.onSceneGUIDelegate += OnSceneGUIDelegate;
#elif UNITY_2019_1_OR_NEWER
		SceneView.duringSceneGui += OnSceneGUIDelegate;
#endif
		}

		private static void OnSceneGUIDelegate(SceneView sceneview)
		{
			Event e = Event.current;
			if (e.type == EventType.KeyDown)
			{
				switch (e.keyCode)
				{
					case KeyCode.N:
						if (e.alt)
						{
							AssetsMenuItem.CopyName();
							e.Use();
						}

						break;
					case KeyCode.M:
						if (e.alt)
						{
							PasteName();
							e.Use();
						}

						break;
					case KeyCode.Space:
						ToggleGameObjcetActiveSelf();
						e.Use();
						break;
				}
			}
		}

		private static void ProjectWindowItemOnGui(string guid, Rect selectionRect)
		{
			Event e = Event.current;
			if (e.type == EventType.KeyDown)
			{
				switch (e.keyCode)
				{
					case KeyCode.N:
						if (e.alt)
						{
							AssetsMenuItem.CopyName();
							e.Use();
						}

						break;
				}
			}
			else if (e.type == EventType.MouseDown
			         && e.button == 2
			         && selectionRect.Contains(e.mousePosition))
			{
				ProcessingPath(e.alt, AssetDatabase.GUIDToAssetPath(guid));
				e.Use();
			}
		}

		private static void ProcessingPath(bool alt, string strPath)
		{
			if (alt)
			{
				GUIUtility.systemCopyBuffer = strPath;
				Debug.Log(strPath);
				return;
			}

			if (Path.GetExtension(strPath) == string.Empty) //文件夹
			{
				Process.Start(Path.GetFullPath(strPath));
			}
			else //文件
			{
				Process.Start("explorer.exe", "/select," + Path.GetFullPath(strPath));
			}
		}

		private static List<Transform> list = new List<Transform>(64);
		private static bool changeSiblingIndex;
		private static Transform activeTransform;

		private static void HierarchyWindowItemOnGui(int instanceID, Rect selectionRect)
		{
			if (Selection.gameObjects.Length == 0) { return; }

			void MoveTargetAnchoredPosition(Vector2 v2Unit, IEnumerable<Transform> targets)
			{
				foreach (Transform item in targets)
				{
					RectTransform rtf = item as RectTransform;
					if (rtf) { rtf.anchoredPosition += v2Unit; }
				}
			}

			Event e = Event.current;
			if (e.type == EventType.KeyDown)
			{
				if (e.control)
				{
					int nUnit = e.shift ? 10 : 1;
					switch (e.keyCode)
					{
						case KeyCode.UpArrow:
							Undo.RecordObjects(Selection.transforms, "UpArrow");
							MoveTargetAnchoredPosition(Vector2.up * nUnit, Selection.transforms);
							e.Use();
							break;
						case KeyCode.DownArrow:
							Undo.RecordObjects(Selection.transforms, "DownArrow");
							MoveTargetAnchoredPosition(Vector2.down * nUnit, Selection.transforms);
							e.Use();
							break;
						case KeyCode.LeftArrow:
							Undo.RecordObjects(Selection.transforms, "LeftArrow");
							MoveTargetAnchoredPosition(Vector2.left * nUnit, Selection.transforms);
							e.Use();
							break;
						case KeyCode.RightArrow:
							Undo.RecordObjects(Selection.transforms, "RightArrow");
							MoveTargetAnchoredPosition(Vector2.right * nUnit, Selection.transforms);
							e.Use();
							break;
					}
				}

				if (e.alt)
				{
					Transform newActiveTransform = Selection.activeTransform;
					if (activeTransform != newActiveTransform)
					{
						if (activeTransform && changeSiblingIndex)
						{
							changeSiblingIndex = false;
							if (activeTransform.parent == newActiveTransform.parent)
							{
								int s1 = activeTransform.GetSiblingIndex();
								int s2 = newActiveTransform.GetSiblingIndex();
								Undo.SetTransformParent(activeTransform, activeTransform.parent, "Set sibling");
								activeTransform.SetSiblingIndex(s1 < s2 ? s2 : s2 + 1);
							}

							return;
						}

						activeTransform = newActiveTransform;
					}


					switch (e.keyCode)
					{
						case KeyCode.N:
							AssetsMenuItem.CopyName();
							e.Use();
							break;
						case KeyCode.M:
							PasteName();
							e.Use();
							break;
						case KeyCode.DownArrow:
							if (e.alt)
							{
								if (!AddSelectionTransformsBySameParent()) { return; }

								list.Sort((t1, t2) => t2.GetSiblingIndex() - t1.GetSiblingIndex());
								for (int i = 0; i < list.Count; i++)
								{
									Transform transform = list[i];
									int target = transform.GetSiblingIndex() + 1;
									if (transform.parent)
									{
										if (target >= transform.parent.childCount) { target = 0; }
									}

									Undo.SetTransformParent(transform, transform.parent, "Set sibling");
									transform.SetSiblingIndex(target);
								}

								e.Use();
							}

							break;
						case KeyCode.UpArrow:
							if (e.alt)
							{
								if (!AddSelectionTransformsBySameParent()) { return; }

								list.Sort((t1, t2) => t1.GetSiblingIndex() - t2.GetSiblingIndex());
								for (int i = 0; i < list.Count; i++)
								{
									Transform transform = list[i];
									int target = transform.GetSiblingIndex() - 1;
									if (target < 0 && !transform.parent) { continue; }

									Undo.SetTransformParent(transform, transform.parent, "Set sibling");
									transform.SetSiblingIndex(target);
								}

								e.Use();
							}

							break;
					}
				}

				switch (e.keyCode)
				{
					case KeyCode.Space:
						ToggleGameObjcetActiveSelf();
						e.Use();
						break;
				}
			}
			else if (e.type == EventType.MouseDown)
			{
				if (e.button == 2)
				{
					if (e.alt)
					{
						StringBuilder sb = new StringBuilder(256);
						for (int i = 0; i < Selection.gameObjects.Length; i++)
						{
							GameObject gameObject = Selection.gameObjects[i];
							sb.AppendLine(gameObject.transform.GetPathExt());
						}

						sb.Remove(sb.Length - 1, 1);
						string paths = sb.ToString();
						GUIUtility.systemCopyBuffer = paths;
						Debug.Log(paths);
					}
					else if (e.shift) { ToggleGameObjcetActive(); }
					else { ToggleGameObjcetActiveSelf(); }

					e.Use();
				}

				if (e.button == 0)
				{
					if (e.alt)
					{
						changeSiblingIndex = true;
						return;
					}
				}

				changeSiblingIndex = false;
			}
		}

		private static bool AddSelectionTransformsBySameParent()
		{
			Transform parent = Selection.activeTransform.parent;
			for (int i = 0; i < Selection.transforms.Length; i++)
			{
				Transform transform = Selection.transforms[i];
				if (transform.parent != parent) { return false; }
			}

			list.Clear();
			list.AddRange(Selection.transforms);
			return true;
		}

		private static void PasteName()
		{
			Undo.RecordObjects(Selection.gameObjects, "Name");
			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				GameObject gameObject = Selection.gameObjects[i];
				gameObject.name = GUIUtility.systemCopyBuffer;
			}
		}

		internal static void ToggleGameObjcetActiveSelf()
		{
			Undo.RecordObjects(Selection.objects, "Active");
			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				GameObject gameObject = Selection.gameObjects[i];
				gameObject.SetActive(!gameObject.activeSelf);
			}
		}

		internal static void ToggleGameObjcetActive()
		{
			Undo.RecordObjects(Selection.objects, "Active");
			bool active = Selection.gameObjects[0].activeSelf;
			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				GameObject gameObject = Selection.gameObjects[i];
				gameObject.SetActive(!active);
			}
		}
	}
}