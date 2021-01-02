using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace UI.Xml
{
    [CustomEditor(typeof(XmlLayoutLocalization))]
    public class LocalizationDictionaryInspector : Editor
    {
        private XmlLayoutLocalization.LocalizationDictionary _strings;
        private XmlLayoutLocalization.LocalizationDictionary strings
        {
            get
            {
                if (_strings == null) _strings = ((XmlLayoutLocalization)target).strings;
                return _strings;
            }
        }

        public override void OnInspectorGUI()
        {
            var style = new GUIStyle(EditorStyles.toolbar);
            style.alignment = TextAnchor.MiddleCenter;

            XmlLayoutLocalization.LocalizationDictionary tempDictionary = new XmlLayoutLocalization.LocalizationDictionary();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(24));
            EditorGUILayout.LabelField("Key", style, GUILayout.Width(200));
            EditorGUILayout.LabelField("Value", style);
            EditorGUILayout.EndHorizontal();

            foreach (var kvp in strings)
            {
                EditorGUILayout.BeginHorizontal();

                bool remove = (GUILayout.Button("X", GUILayout.Width(24)));

                var newKey = EditorGUILayout.TextField(kvp.Key, GUILayout.Width(200));
                var newValue = EditorGUILayout.TextField(kvp.Value);

                if (!remove)
                {
                    tempDictionary.Add(newKey, newValue);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(24));
            if (GUILayout.Button("Add New String", GUILayout.Width(200)))
            {
                strings.Add("", "");
                tempDictionary.Add("", "");
            }            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(24));
            if (GUILayout.Button("Copy Keys to Clipboard", GUILayout.Width(200)))
            {
                EditorGUIUtility.systemCopyBuffer = "xmlLayoutLocalizationKeys=" + String.Join(",", strings.Select(k => k.Key).ToArray());
            }
            
            EditorGUI.BeginDisabledGroup(!EditorGUIUtility.systemCopyBuffer.StartsWith("xmlLayoutLocalizationKeys="));            
            if (GUILayout.Button("Paste Keys from Clipboard"))
            {                
                var keysString = EditorGUIUtility.systemCopyBuffer.Replace("xmlLayoutLocalizationKeys=", "");
                var keys = keysString.Split(',').ToList();

                bool confirmed = true;
                if (strings.Any())
                {
                    confirmed = EditorUtility.DisplayDialog("Override existing keys?", "This will replace any existing keys. Are you sure you wish to continue?", "Continue", "Cancel");
                }

                if (confirmed)
                {
                    tempDictionary.Clear();
                    foreach (var key in keys)
                    {
                        tempDictionary.Add(key, strings.ContainsKey(key) ? strings[key] : "");
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                strings.Clear();
                foreach (var kvp in tempDictionary)
                {
                    strings.Add(kvp.Key, kvp.Value);
                }

                EditorUtility.SetDirty(target);                
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(24));
            if (GUILayout.Button("Save Changes", GUILayout.Width(200)))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();

                // update any XmlLayout instances using this localization file
                XmlLayout[] layouts = GameObject.FindObjectsOfType<XmlLayout>();
                foreach (var layout in layouts)
                {
                    if (layout.LocalizationFile == this.target)
                    {
                        layout.RebuildLayout(true, false);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
