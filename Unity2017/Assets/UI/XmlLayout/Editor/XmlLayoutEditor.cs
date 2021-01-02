using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace UI.Xml
{
    [CustomEditor(typeof(XmlLayout))]
    public class XmlLayoutEditor : Editor
    {
        private SerializedObject SO_Target;
        private XmlLayout XmlLayout;

        private SerializedProperty XmlFile;
        private SerializedProperty AutomaticallyReloadXmlFileIfItChanges;
        private SerializedProperty Xml;
        private SerializedProperty DefaultsFiles;

        private SerializedProperty ForceRebuildOnAwake;
        private SerializedProperty ForceReloadXmlFileOnAwake;

        private SerializedProperty LocalizationFile;

        private SerializedProperty editor_showXml;
        private SerializedProperty editor_xmlScrollPosition;
        private SerializedProperty editor_showLocalization;
        
        private bool showUpdateMessage = false;

        private bool showIntellisense = false;

        public void OnEnable()
        {
            XmlLayout = (XmlLayout)target;
            SO_Target = new SerializedObject(target);

            XmlFile = SO_Target.FindProperty("XmlFile");
            AutomaticallyReloadXmlFileIfItChanges = SO_Target.FindProperty("AutomaticallyReloadXmlFileIfItChanges");
            Xml = SO_Target.FindProperty("Xml");
            DefaultsFiles = SO_Target.FindProperty("DefaultsFiles");

            ForceRebuildOnAwake = SO_Target.FindProperty("ForceRebuildOnAwake");
            ForceReloadXmlFileOnAwake = SO_Target.FindProperty("ForceReloadXmlFileOnAwake");

            LocalizationFile = SO_Target.FindProperty("LocalizationFile");

            editor_showXml = SO_Target.FindProperty("editor_showXml");
            editor_xmlScrollPosition = SO_Target.FindProperty("editor_xmlScrollPosition");
            editor_showLocalization = SO_Target.FindProperty("editor_showLocalization");
        }

        public void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            bool reloadXmlFile = false;
            bool update = false;
            bool saveChangesToFile = false;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Force Rebuild On Awake");
            ForceRebuildOnAwake.boolValue = EditorGUILayout.Toggle(ForceRebuildOnAwake.boolValue, GUILayout.Width(32));
            if (GUILayout.Button("Force Rebuild Now", GUILayout.Width(128)))
            {
                XmlLayout.RebuildLayout(true);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            var previousValue = XmlFile.objectReferenceValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(XmlFile);
            if (EditorGUI.EndChangeCheck())
            {
                var textAsset = XmlFile.objectReferenceValue as TextAsset;
                if (textAsset != null)
                {
                    var text = textAsset.text.Trim();
                    if (text.IndexOf("<XmlLayout", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        Debug.LogWarning("Please select a valid XmlLayout Xml File.");
                        // I'd use the Dialog here, but closing the dialog closes the "Select Asset" window as well :/
                        //UnityEditor.EditorUtility.DisplayDialog("Invalid XmlLayout File", "Please select a valid XmlLayout Xml File.", "Okay");

                        // revert the change
                        XmlFile.objectReferenceValue = previousValue;
                    }
                    else
                    {
                        reloadXmlFile = true;
                    }
                }
            }
            else
            {
                if (XmlFile.objectReferenceValue != null) reloadXmlFile = GUILayout.Button("Reload File", GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            if (XmlFile.objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(" ");
                EditorGUILayout.LabelField("Automatically Rebuild if Xml File Changes");
                AutomaticallyReloadXmlFileIfItChanges.boolValue = EditorGUILayout.Toggle(AutomaticallyReloadXmlFileIfItChanges.boolValue);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(" ");
                if (!ForceRebuildOnAwake.boolValue)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    ForceReloadXmlFileOnAwake.boolValue = false;
                }
                EditorGUILayout.LabelField("Force Reload Xml File On Awake");
                ForceReloadXmlFileOnAwake.boolValue = EditorGUILayout.Toggle(ForceReloadXmlFileOnAwake.boolValue);
                if (!ForceRebuildOnAwake.boolValue)
                {
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(" ");
                if (GUILayout.Button("Open Xml File in External Editor"))
                {
                    AssetDatabase.OpenAsset(XmlFile.objectReferenceValue);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(DefaultsFiles, true);
            if (EditorGUI.EndChangeCheck())
            {
                reloadXmlFile = true;
            }

            editor_showXml.boolValue = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), editor_showXml.boolValue, "Xml", true);
            if (editor_showXml.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                editor_xmlScrollPosition.vector2Value = EditorGUILayout.BeginScrollView(editor_xmlScrollPosition.vector2Value, false, false, GUILayout.Height(400));
                Xml.stringValue = EditorGUILayout.TextArea(Xml.stringValue, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                if (EditorGUI.EndChangeCheck())
                {
                    showUpdateMessage = true;
                }

                if (showUpdateMessage)
                {
                    EditorGUILayout.HelpBox("Click the 'Update Element' button to apply any changes from editing the Xml.", MessageType.Info);
                }

                if (!showUpdateMessage) EditorGUI.BeginDisabledGroup(true);
                update = GUILayout.Button("Update Element");
                if (!showUpdateMessage) EditorGUI.EndDisabledGroup();
                if (XmlFile.objectReferenceValue as TextAsset != null)
                {
                    saveChangesToFile = GUILayout.Button("Save Changes to Xml File");
                }
            }

            editor_showLocalization.boolValue = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), editor_showLocalization.boolValue, "Localization", true);
            if (editor_showLocalization.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(LocalizationFile);
                if (EditorGUI.EndChangeCheck())
                {
                    XmlLayoutTimer.DelayedCall(0, () => { XmlLayout.RebuildLayout(true, false); }, XmlLayout);
                }
            }

            if (GUI.changed)
            {
                SO_Target.ApplyModifiedProperties();
            }

            if (reloadXmlFile)
            {
                showUpdateMessage = false;
                XmlLayout.ReloadXmlFile();

                Xml.stringValue = XmlLayout.Xml;
                SO_Target.ApplyModifiedProperties();
            }
            else
            {
                if (update)
                {
                    showUpdateMessage = false;
                    XmlLayout.RebuildLayout();
                }

                if (saveChangesToFile)
                {
                    showUpdateMessage = false;

                    var path = AssetDatabase.GetAssetPath(XmlLayout.XmlFile);
                    File.WriteAllText(path, Xml.stringValue);

                    AssetDatabase.Refresh();
                    XmlLayoutEditorUtilities.XmlFileUpdated(path);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            showIntellisense = EditorGUILayout.Foldout(showIntellisense, "Intellisense");

            if (showIntellisense)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Edit XSD File");

                if (GUILayout.Button("In Visual Studio"))
                {
                    AssetDatabase.OpenAsset(XmlLayoutUtilities.XmlLayoutConfiguration.XSDFile);
                }
                if (GUILayout.Button("In MonoDevelop"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(AssetDatabase.GetAssetPath(XmlLayoutUtilities.XmlLayoutConfiguration.XSDFile), 0);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
