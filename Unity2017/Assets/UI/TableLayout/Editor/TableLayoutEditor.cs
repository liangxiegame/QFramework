using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace UI.Tables
{
    [CustomEditor(typeof(TableLayout))]
    public class TableLayoutEditor : Editor
    {
        private SerializedObject SO_Target;
        private TableLayout tableLayout;


        private SerializedProperty Padding;
        private SerializedProperty RowBackgroundImage;
        private SerializedProperty RowBackgroundColor;
        private SerializedProperty CellBackgroundImage;
        private SerializedProperty CellBackgroundColor;

        private SerializedProperty AutomaticallyAddColumns;
        private SerializedProperty AutomaticallyRemoveEmptyColumns;

        private SerializedProperty ColumnWidths;

        private SerializedProperty UseGlobalCellPadding;
        private SerializedProperty CellPadding;
        private SerializedProperty CellSpacing;

        private SerializedProperty AutoCalculateHeight;

        public void OnEnable()
        {
            SO_Target = new SerializedObject(target);
            tableLayout = (TableLayout)target;

            Padding = SO_Target.FindProperty("m_Padding");
            RowBackgroundImage = SO_Target.FindProperty("RowBackgroundImage");
            RowBackgroundColor = SO_Target.FindProperty("RowBackgroundColor");
            CellBackgroundImage = SO_Target.FindProperty("CellBackgroundImage");
            CellBackgroundColor = SO_Target.FindProperty("CellBackgroundColor");

            AutomaticallyAddColumns = SO_Target.FindProperty("AutomaticallyAddColumns");
            AutomaticallyRemoveEmptyColumns = SO_Target.FindProperty("AutomaticallyRemoveEmptyColumns");

            ColumnWidths = SO_Target.FindProperty("ColumnWidths");

            UseGlobalCellPadding = SO_Target.FindProperty("UseGlobalCellPadding");
            CellPadding = SO_Target.FindProperty("CellPadding");
            CellSpacing = SO_Target.FindProperty("CellSpacing");

            AutoCalculateHeight = SO_Target.FindProperty("AutoCalculateHeight");
        }

        public void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            EditorGUILayout.PropertyField(Padding, true);
            EditorGUILayout.PropertyField(RowBackgroundImage);
            EditorGUILayout.PropertyField(RowBackgroundColor);
            EditorGUILayout.PropertyField(CellBackgroundImage);
            EditorGUILayout.PropertyField(CellBackgroundColor);

            EditorGUILayout.PropertyField(AutomaticallyAddColumns);
            EditorGUILayout.PropertyField(AutomaticallyRemoveEmptyColumns);
            EditorGUILayout.PropertyField(ColumnWidths, true);

            EditorGUILayout.PropertyField(UseGlobalCellPadding);
            if (!UseGlobalCellPadding.boolValue) EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(CellPadding, true);
            if (!UseGlobalCellPadding.boolValue) EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(CellSpacing);
            EditorGUILayout.PropertyField(AutoCalculateHeight);

            GUILayout.Space(16);

            if (GUILayout.Button(String.Format("Add row with {0} empty cells", tableLayout.ColumnWidths.Count)))
            {
                tableLayout.AddRow(tableLayout.ColumnWidths.Count);
            }

            if (GUILayout.Button("Add Empty Row"))
            {
                tableLayout.AddRow(0);
            }

            EditorGUI.BeginDisabledGroup(tableLayout.Rows.Count <= 0);
            if (GUILayout.Button("Duplicate Last Row"))
            {
                var lastRow = tableLayout.Rows.Last();
                var newRow = GameObject.Instantiate(lastRow);
                newRow.name = lastRow.name;

                newRow.transform.SetParent(tableLayout.transform);
                newRow.transform.SetAsLastSibling();
            }
            EditorGUI.EndDisabledGroup();

            SO_Target.ApplyModifiedProperties();
        }
    }
}
