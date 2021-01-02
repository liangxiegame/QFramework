using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UI.Xml.Configuration
{
    [CustomEditor(typeof(XmlLayoutConfiguration))]
    public class XmlConfigurationEditor : Editor
    {              
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Regenerate XSD file Now"))
            {
                XmlSchemaProcessor.ProcessXmlSchema(true);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            if (GUILayout.Button("Edit XSD file in Visual Studio"))            
            {
                AssetDatabase.OpenAsset(((XmlLayoutConfiguration)target).XSDFile);
            }

            if (GUILayout.Button("Edit XSD file in MonoDevelop"))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(AssetDatabase.GetAssetPath(((XmlLayoutConfiguration)target).XSDFile), 0);                
            }
        }
    }
}
