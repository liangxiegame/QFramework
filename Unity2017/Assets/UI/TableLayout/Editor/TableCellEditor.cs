using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace UI.Tables
{
    [CustomEditor(typeof(TableCell))]
    public class TableCellEditor : Editor
    {
        private SerializedObject SO_Target;
        private TableCell TableCell;

        private SerializedProperty columnSpan;
        private SerializedProperty dontUseTableCellBackground;
        private SerializedProperty overrideGlobalPadding;
        private SerializedProperty padding;        

        public void OnEnable()
        {
            SO_Target = new SerializedObject(target);
            TableCell = (TableCell)target;

            columnSpan = SO_Target.FindProperty("columnSpan");
            dontUseTableCellBackground = SO_Target.FindProperty("dontUseTableCellBackground");
            overrideGlobalPadding = SO_Target.FindProperty("overrideGlobalPadding");
            padding = SO_Target.FindProperty("m_Padding");
        }

        public void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(columnSpan);
            EditorGUILayout.PropertyField(dontUseTableCellBackground);            
            EditorGUILayout.PropertyField(overrideGlobalPadding);
            if (!overrideGlobalPadding.boolValue)
            {
                EditorGUI.BeginDisabledGroup(true);                
            }
            EditorGUILayout.PropertyField(padding, true);
            if (!overrideGlobalPadding.boolValue) EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {                                
                SO_Target.ApplyModifiedProperties();

                TableCell.columnSpan = columnSpan.intValue;
                TableCell.dontUseTableCellBackground = dontUseTableCellBackground.boolValue;
                TableCell.overrideGlobalPadding = overrideGlobalPadding.boolValue;

                if (!TableCell.overrideGlobalPadding)
                {
                    TableCell.SetCellPaddingFromTableLayout();
                }

                Repaint();           
            }

            TableCell.NotifyTableCellPropertiesChanged();
        }
    }
}
