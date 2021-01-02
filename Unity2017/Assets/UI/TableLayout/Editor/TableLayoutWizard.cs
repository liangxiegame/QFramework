using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace UI.Tables
{    
    public class TableLayoutWizard : EditorWindow
    {
        int numberOfRows = 3;
        int numberOfColumns = 3;


        [MenuItem("GameObject/UI/TableLayout/Add New Table")]
        static void AddTableLayout()
        {
            var window = EditorWindow.GetWindow<TableLayoutWizard>();
            window.Show();

            var width = 380f;
            var height = 110f;            

            window.titleContent = new GUIContent("Add New TableLayout");
            window.position = new Rect( (Screen.currentResolution.width - width) / 2f, 
                                        (Screen.currentResolution.height - height) / 2f,
                                        width,
                                        height);                                  
        }

        void OnGUI()
        {
            var style = new GUIStyle();
            style.margin = new RectOffset(10, 10, 10, 10);

            GUILayout.BeginVertical(style);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Number of Rows");
            numberOfRows = EditorGUI.IntSlider(EditorGUILayout.GetControlRect(), numberOfRows, 0, 32);
            GUILayout.EndHorizontal();

            if (numberOfRows == 0) EditorGUI.BeginDisabledGroup(true);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Number of Columns");
            numberOfColumns = EditorGUI.IntSlider(EditorGUILayout.GetControlRect(), numberOfColumns, 0, 32);
            GUILayout.EndHorizontal();
            if (numberOfRows == 0) EditorGUI.EndDisabledGroup();

            GUILayout.Space(16);

            if (GUILayout.Button("Add Table Layout"))
            {
                CreateTable(numberOfRows, numberOfColumns);
                this.Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                this.Close();
            }
            GUILayout.EndVertical();
        }

        void CreateTable(int rows, int columns)
        {
            var gameObject = TableLayoutUtilities.InstantiatePrefab("TableLayout/TableLayout");
            gameObject.name = "TableLayout";

            var tableLayout = gameObject.GetComponent<TableLayout>();

            for (var x = 0; x < rows; x++)
            {
                tableLayout.AddRow(columns);
            }

            UnityEditor.Selection.activeObject = gameObject;
        }
    }
}
