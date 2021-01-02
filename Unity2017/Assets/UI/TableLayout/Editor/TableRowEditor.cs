using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace UI.Tables
{
    [CustomEditor(typeof(TableRow)), CanEditMultipleObjects]
    public class TableRowEditor : Editor
    {
        //private SerializedObject serializedObject;
        private TableRow TableRow;

        private SerializedProperty preferredHeight;
        private SerializedProperty dontUseTableRowBackground;
              

        public void OnEnable()
        {
            //serializedObject = new SerializedObject(target);
            TableRow = (TableRow)target;

            preferredHeight = serializedObject.FindProperty("preferredHeight");
            dontUseTableRowBackground = serializedObject.FindProperty("dontUseTableRowBackground");
            
        }

        public void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(preferredHeight);
            EditorGUILayout.PropertyField(dontUseTableRowBackground);                        

            if (EditorGUI.EndChangeCheck())
            {                                
                serializedObject.ApplyModifiedProperties();

                TableRow.preferredHeight = preferredHeight.floatValue;
                TableRow.dontUseTableRowBackground = dontUseTableRowBackground.boolValue;                                

                Repaint();           
            }

            if (GUILayout.Button("Add Cell"))
            {
                TableRow.AddCell();
            }

            TableRow.NotifyTableRowPropertiesChanged();
        }
    }
}
