#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

// TODO:
namespace QFramework
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// </summary>
        public static int GetUniquePropertyId(this SerializedProperty property) 
            => property.serializedObject.targetObject.GetType().GetHashCode() 
               + property.propertyPath.GetHashCode();
        
        
        public static void DrawProperties(this SerializedObject serializedObject, bool drawScript = true,
            int indent = 0,
            params string[] ignorePropertyNames)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (drawScript)
                {
                    using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    {
                        if (!ignorePropertyNames.Contains(iterator.propertyPath))
                        {
                            iterator.DrawProperty(indent);
                        }
                    }
                }
                else
                {
                    if (iterator.propertyPath != "m_Script" && !ignorePropertyNames.Contains(iterator.propertyPath))
                    {
                        iterator.DrawProperty(indent);
                    }
                }
            }
        }

        public static void DrawProperty(this SerializedProperty property, int indent = 0)
        {
            if (indent == 0)
            {
                EditorGUILayout.PropertyField(property, true);
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(indent);
                EditorGUILayout.PropertyField(property, true);
                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif