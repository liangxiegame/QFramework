using System.IO;
using Unidux.Util;
using UnityEditor;
using UnityEngine;

namespace Unidux.Experimental.Editor
{
    public class UniduxPanelSettingTab
    {
        private const string DefaultStateJsonPath = "Assets/state.json";
        private const string JsonPathKey = "UniduxPanel.JsonSavePath";
        
        public void Render(IStoreAccessor _store, ISerializeFactory serializer)
        {
            if (_store == null)
            {
                EditorGUILayout.HelpBox("Please Set IStoreAccessor", MessageType.Warning);
                return;
            }

            var title = "State Json:";
            var jsonPath = EditorPrefs.GetString(JsonPathKey, DefaultStateJsonPath);
            var existsJson = File.Exists(jsonPath);

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (existsJson)
            {
                if (GUILayout.Button(jsonPath, EditorStyles.label))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
                    Selection.activeObject = asset;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Create or Select json to save/load", MessageType.Warning);
            }

            EditorGUILayout.BeginHorizontal();
            if (!existsJson && GUILayout.Button("Create"))
            {
                jsonPath = EditorUtility.SaveFilePanel("Create empty json", "Assets", "state.json", "json");
                jsonPath = ReplaceJsonPath(jsonPath);
                if (!string.IsNullOrEmpty(jsonPath))
                {
                    File.WriteAllText(jsonPath, "{}");
                    AssetDatabase.Refresh();
                    RecordJsonPath(jsonPath);
                }
            }

            if (GUILayout.Button("Select"))
            {
                jsonPath = EditorUtility.OpenFilePanel("Select json to save", "", "json");
                jsonPath = ReplaceJsonPath(jsonPath);
                RecordJsonPath(jsonPath);
            }

            if (existsJson && GUILayout.Button("Save"))
            {
                var json = serializer.Serialize(_store.StoreObject.ObjectState);
                File.WriteAllBytes(jsonPath, json);
                AssetDatabase.Refresh();
            }

            if (existsJson && GUILayout.Button("Load"))
            {
                var content = File.ReadAllBytes(jsonPath);
                _store.StoreObject.ObjectState = serializer.Deserialize(content, _store.StoreObject.StateType);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private string ReplaceJsonPath(string path)
        {
            var projectPath = Application.dataPath.Replace("Assets", "");
            return path.Replace(projectPath, "");
        }
        
        private void RecordJsonPath(string path)
        {
            EditorPrefs.SetString(JsonPathKey, path);
        }
    }
}