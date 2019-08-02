using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MiniJSON;
using UnityEditor;
using UnityEngine;

namespace Unidux.Experimental.Editor
{
    [CustomEditor(typeof(StateJsonFileWrapper))]
    public class StateJsonFileWrapperInspector : UnityEditor.Editor
    {
        private Vector2 leftScrollPos = Vector2.zero;
        private Dictionary<string, bool> foldoutMap = new Dictionary<string, bool>();
        private DateTime lastUpdatedAt = new DateTime(1970, 1, 1);
        private string fileText = null;
        private object fileJson = null;

        public override void OnInspectorGUI()
        {
            StateJsonFileWrapper wrapper = (StateJsonFileWrapper) target;

            RenderHeader(wrapper);

            if (!File.Exists(wrapper.FileName)) return;

            // Display Json
            bool dirty = false;
            this.leftScrollPos = EditorGUILayout.BeginScrollView(this.leftScrollPos, GUI.skin.box);
            {
                var updatedAt = File.GetLastWriteTimeUtc(wrapper.FileName);
                if (this.fileText == null || updatedAt.CompareTo(this.lastUpdatedAt) > 0)
                {
                    this.lastUpdatedAt = updatedAt;
                    this.fileText = File.ReadAllText(wrapper.FileName);
                    this.fileJson = Json.Deserialize(this.fileText);
                }

                if (this.fileJson is Dictionary<string, object>)
                {
                    dirty = RenderJsonDictionary(new string[0] { }, this.fileJson as Dictionary<string, object>);
                }
                else if (this.fileJson is List<object>)
                {
                    dirty = RenderJsonArray(new string[0] { }, "", this.fileJson as List<object>);
                }
                else
                {
                    EditorGUILayout.LabelField(this.fileText);
                }
            }
            EditorGUILayout.EndScrollView();

            if (dirty)
            {
                string content = Json.Serialize(this.fileJson);
                File.WriteAllText(wrapper.FileName, content);
                AssetDatabase.Refresh();
            }
        }

        private void RenderHeader(StateJsonFileWrapper wrapper)
        {
            EditorGUILayout.LabelField("Shown by Unidux.StateJsonEditor", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(GUIStyle.none);
            EditorGUILayout.LabelField(wrapper.FileName, EditorStyles.miniLabel);

            if (File.Exists("/usr/bin/open"))
            {
                if (GUILayout.Button("OpenFile", EditorStyles.miniButton))
                {
                    System.Diagnostics.Process.Start("/usr/bin/open", wrapper.FileName);
                }
                if (GUILayout.Button("OpenDir", EditorStyles.miniButton))
                {
                    System.Diagnostics.Process.Start("/usr/bin/open", Path.GetDirectoryName(wrapper.FileName));
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool RenderJsonDictionary(string[] parentsKey, Dictionary<string, object> dict)
        {
            bool dirty = false;

            if (dict == null || dict.Count <= 0) return dirty;

            foreach (var key in dict.Keys.ToArray())
            {
                object _value = dict[key];
                if (_value is string)
                {
                    // EditorGUILayout.LabelField(dict[key] as string);
                    string value = _value as string;
                    string newValue = EditorGUILayout.DelayedTextField(key, value);

                    if (newValue != value)
                    {
                        dict[key] = newValue;
                        dirty = true;
                    }
                }
                else if (_value is long)
                {
                    // XXX: There are no DelayedLongField
                    long __value = (long) _value;
                    if (__value > int.MaxValue || __value < int.MinValue)
                    {
                        Debug.LogWarning("StateJsonEditor not handled long size value => " + __value);
                    }

                    int value = (int) __value;
                    int newValue = EditorGUILayout.DelayedIntField(key, value);

                    if (newValue != value)
                    {
                        dict[key] = newValue;
                        dirty = true;
                    }
                }
                else if (_value is double)
                {
                    double value = (double) _value;
                    double newValue = EditorGUILayout.DelayedDoubleField(key, value);

                    if (newValue != value)
                    {
                        dict[key] = newValue;
                        dirty = true;
                    }
                }
                else if (_value is bool)
                {
                    bool value = (bool) _value;
                    bool newValue = EditorGUILayout.Toggle(key, value);

                    if (newValue != value)
                    {
                        dict[key] = newValue;
                        dirty = true;
                    }
                }
                else if (_value == null)
                {
                    EditorGUILayout.LabelField(key, "null");
                }
                else if (_value is List<object>)
                {
                    string[] newParentsKey = CreateParentsKey(parentsKey, key);
                    string foldoutKey = CreateFoldoutKey(newParentsKey);
                    string displayName = CreateDisplayKey(newParentsKey);

                    if (ShouldFoldout(displayName, foldoutKey))
                    {
                        dirty = RenderJsonArray(newParentsKey, key, (List<object>) _value);
                    }
                }
                else if (_value is Dictionary<string, object>)
                {
                    string[] newParentsKey = CreateParentsKey(parentsKey, key);
                    string foldoutKey = CreateFoldoutKey(newParentsKey);
                    string displayName = CreateDisplayKey(newParentsKey);

                    if (ShouldFoldout(displayName, foldoutKey))
                    {
                        dirty = RenderJsonDictionary(newParentsKey, (Dictionary<string, object>) _value);
                    }
                }
                else
                {
                    Debug.LogWarning("Unknown type of json: key => " + key + ", value => " + _value);
                }
            }

            return dirty;
        }

        private bool RenderJsonArray(string[] parentsKey, string arrayKey, List<object> array)
        {
            bool dirty = false;

            if (array.Count <= 0) return dirty;

            for (int i = 0; i < array.Count; i++)
            {
                var key = new StringBuilder(arrayKey).Append("[").Append(i).Append("]").ToString();
                var _value = array[i];
                if (_value is string)
                {
                    string value = _value as string;
                    string newValue = EditorGUILayout.DelayedTextField(key, value);
                    if (newValue != value)
                    {
                        array[i] = newValue;
                        dirty = true;
                    }
                }
                else if (_value is long)
                {
                    // XXX: There are no DelayedLongField
                    long __value = (long) _value;
                    if (__value > int.MaxValue || __value < int.MinValue)
                    {
                        Debug.LogWarning("StateJsonEditor not handled long size value => " + __value);
                    }

                    int value = (int) __value;
                    int newValue = EditorGUILayout.DelayedIntField(key, value);

                    if (newValue != value)
                    {
                        array[i] = newValue;
                        dirty = true;
                    }
                }
                else if (_value is double)
                {
                    double value = (double) _value;
                    double newValue = EditorGUILayout.DelayedDoubleField(key, value);

                    if (newValue != value)
                    {
                        array[i] = newValue;
                        dirty = true;
                    }
                }
                else if (_value is bool)
                {
                    bool value = (bool) _value;
                    bool newValue = EditorGUILayout.Toggle(key, value);

                    if (newValue != value)
                    {
                        array[i] = newValue;
                        dirty = true;
                    }
                }
                else if (_value == null)
                {
                    EditorGUILayout.LabelField(key, "null");
                }
                else if (_value is List<object>)
                {
                    string[] newParentsKey = CreateParentsKey(parentsKey, key);
                    string foldoutKey = CreateFoldoutKey(newParentsKey);
                    string displayName = CreateDisplayKey(newParentsKey);

                    if (ShouldFoldout(displayName, foldoutKey))
                    {
                        dirty = RenderJsonArray(newParentsKey, key, _value as List<object>);
                    }
                }
                else if (_value is Dictionary<string, object>)
                {
                    string[] newParentsKey = CreateParentsKey(parentsKey, key);
                    string foldoutKey = CreateFoldoutKey(newParentsKey);
                    string displayName = CreateDisplayKey(newParentsKey);

                    if (ShouldFoldout(displayName, foldoutKey))
                    {
                        dirty = RenderJsonDictionary(newParentsKey, _value as Dictionary<string, object>);
                    }
                }
                else
                {
                    Debug.LogWarning("Unknown type of json: key => " + key + ", value => " + _value);
                }
            }

            return dirty;
        }

        private bool CustomFold(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }

        private string[] CreateParentsKey(string[] parentsKey, string key)
        {
            if (parentsKey == null) return new string[] {key};
            var list = new List<string>(parentsKey);
            list.Add(key);
            return list.ToArray();
        }

        private string CreateFoldoutKey(string[] parentsKey)
        {
            return string.Join(">", parentsKey);
        }

        private string CreateDisplayKey(string[] parentsKey)
        {
            if (parentsKey.Length == 1) return parentsKey[0];
            return new StringBuilder(new string('.', parentsKey.Length-1)).Append(parentsKey.LastOrDefault()).ToString();
        }

        private bool ShouldFoldout(string name, string foldoutKey)
        {
            if (!this.foldoutMap.ContainsKey(foldoutKey)) this.foldoutMap[foldoutKey] = false;
            this.foldoutMap[foldoutKey] = CustomFold(name, this.foldoutMap[foldoutKey]);

            return this.foldoutMap[foldoutKey];
        }
    }
}
