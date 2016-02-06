/// <Licensing>
/// ©2011-2014 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
///	Functions to help with custom editors
/// </summary>
public static class PGEditorUtils 
{
    // Constants for major settings
    public const int CONTROLS_DEFAULT_LABEL_WIDTH = 140;
    public const string FOLD_OUT_TOOL_TIP = "Click to Expand/Collapse";



    #region Managment Utilities
    /// <summary>
    /// Will get an asset file at the specified path (and create one if none exists)
    /// The file will be named the same as the type passed with '.asset' appended
    /// </summary>
    /// <param name="basePath">The folder to look in</param>
    /// <returns>A reference to the asset component</returns>
    public static T GetDataAsset<T>(string basePath) where T : ScriptableObject
    {
        string fileName = typeof(T).ToString();
        string filePath = string.Format("{0}/{1}.asset", basePath, fileName);
        T asset;
        asset = (T)AssetDatabase.LoadAssetAtPath(filePath, typeof(T));
        if (!asset)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, filePath);
            asset.hideFlags = HideFlags.HideInHierarchy;
        }

        return asset;
    }

    #endregion Managment Utilities



    #region Layout Utilities
    /// <summary>
    /// Does the same thing as EditorGUIUtility.LookLikeControls but with a defaut 
    /// label width closer to the regular inspector look
    /// </summary>
    public static void SetLabelWidth()
    {
#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
        EditorGUIUtility.LookLikeControls(CONTROLS_DEFAULT_LABEL_WIDTH);
#else
		EditorGUIUtility.labelWidth = CONTROLS_DEFAULT_LABEL_WIDTH;
#endif
	}    
	
	public static void SetLabelWidth(int width)
    {
#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
        EditorGUIUtility.LookLikeControls(width);
#else
		EditorGUIUtility.labelWidth = width;
#endif
	}
	
	// For backwards compatability.
	public static void LookLikeControls()
	{
		SetLabelWidth();
	}
	
	/// <summary>
	/// A toggle button for a Bool type SerializedProperty. Nothing is returned because the 
	/// property is set by reference.
	/// </summary>
	/// <param name='property'>
	/// SerializedProperty.
	/// </param>
	/// <param name='content'>
	/// GUIContent(label, tooltip)
	/// </param>
	/// <param name='width'>
	/// Width of the button
	/// </param>
	public static void ToggleButton(SerializedProperty property, GUIContent content, int width)
	{
		GUIStyle style = new GUIStyle(EditorStyles.miniButton);
        style.alignment = TextAnchor.MiddleCenter;
        style.fixedWidth = width;
		
		// Not sure why we need this return value. Just copied from the Unity docs.
		content = EditorGUI.BeginProperty(new Rect(0, 0, 0, 0), content, property);

		EditorGUI.BeginChangeCheck();
		bool newValue = GUILayout.Toggle(property.boolValue, content, style);

		// Only assign the value back if it was actually changed by the user.
		// Otherwise a single value will be assigned to all objects when multi-object editing,
		// even when the user didn't touch the control.
		if (EditorGUI.EndChangeCheck())
			property.boolValue = newValue;
		
		EditorGUI.EndProperty();
	}
	
    /// <summary>
    /// A generic version of EditorGUILayout.ObjectField.
    /// Allows objects to be drag and dropped or picked.
    /// This version defaults to 'allowSceneObjects = true'.
    /// 
    /// Instead of this:
    ///     var script = (MyScript)target;
    ///     script.xform = (Transform)EditorGUILayout.ObjectField("My Transform", script.xform, typeof(Transform), true);        
    /// 
    /// Do this:    
    ///     var script = (MyScript)target;
    ///     script.xform = EditorGUILayout.ObjectField<Transform>("My Transform", script.xform);        
    /// </summary>
    /// <typeparam name="T">The type of object to use</typeparam>
    /// <param name="label">The label (text) to show to the left of the field</param>
    /// <param name="obj">The obj variable of the script this GUI field is for</param>
    /// <returns>A reference to what is in the field. An object or null.</returns>
    public static T ObjectField<T>(string label, T obj) where T : UnityEngine.Object
    {
        return ObjectField<T>(label, obj, true);
    }

    /// <summary>
    /// A generic version of EditorGUILayout.ObjectField.
    /// Allows objects to be drag and dropped or picked.
    /// </summary>
    /// <typeparam name="T">The type of object to use</typeparam>
    /// <param name="label">The label (text) to show to the left of the field</param>
    /// <param name="obj">The obj variable of the script this GUI field is for</param>
    /// <param name="allowSceneObjects">Allow scene objects. See Unity Docs for more.</param>
    /// <returns>A reference to what is in the field. An object or null.</returns>
    public static T ObjectField<T>(string label, T obj, bool allowSceneObjects)
            where T : UnityEngine.Object
    {
        return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects);
    }

    /// <summary>
    /// A generic version of EditorGUILayout.ObjectField.
    /// Allows objects to be drag and dropped or picked.
    /// </summary>
    /// <typeparam name="T">The type of object to use</typeparam>
    /// <param name="label">The label (text) to show to the left of the field</param>
    /// <param name="obj">The obj variable of the script this GUI field is for</param>
    /// <param name="allowSceneObjects">Allow scene objects. See Unity docs for more.</param>
    /// <param name="options">Layout options. See Unity docs for more.</param>
    /// <returns>A reference to what is in the field. An object or null.</returns>
    public static T ObjectField<T>(string label, T obj, bool allowSceneObjects, GUILayoutOption[] options)
            where T : UnityEngine.Object
    {
        return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
    }

    /// <summary>
    /// A generic version of EditorGUILayout.EnumPopup.
    /// Displays an enum as a pop-up list of options
    /// 
    /// Instead of this:
    ///     var script = (MyScript)target;
    ///     script.options = (MyScript.MY_ENUM_OPTIONS)EditorGUILayout.EnumPopup("Options", (System.Enum)script.options);       
    /// 
    /// Do this:    
    ///     var script = (MyScript)target;
    ///     script.options = EditorGUILayout.EnumPopup<MyScript.MY_ENUM_OPTIONS>("Options", script.options);        
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <param name="label">The label (text) to show to the left of the field</param>
    /// <param name="enumVar">The enum variable of the script this GUI field is for</param>
    /// <returns>The chosen option</returns>
    public static T EnumPopup<T>(string label, T enumVal)
            where T : struct, IFormattable, IConvertible, IComparable
    {
        Enum e;
        if ((e = enumVal as Enum) == null)
            throw new ArgumentException("value must be an Enum");
        object res = EditorGUILayout.EnumPopup(label, e);
        return (T)res;
    }


    /// <summary>
    /// See EnumPopup<T>(string label, T enumVal).
    /// This enables label-less GUI fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumVal"></param>
    /// <returns></returns>
    public static T EnumPopup<T>(T enumVal)
            where T : struct, IFormattable, IConvertible, IComparable
    {
        Enum e;
        if ((e = enumVal as Enum) == null)
            throw new ArgumentException("value must be an Enum");
        object res = EditorGUILayout.EnumPopup(e);
        return (T)res;
    }

    /// <summary>
    /// Add a LayerMash field. This hasn't been made available by Unity even though
    /// it exists in the automated version of the inspector (when no custom editor).
    /// </summary>
    /// <param name="label">The string to display to the left of the control</param>
    /// <param name="selected">The LayerMask variable</param>
    /// <returns>A new LayerMask value</returns>
    public static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        return LayerMaskField(label, selected, true);
    }

    /// <summary>
    /// Add a LayerMash field. This hasn't been made available by Unity even though
    /// it exists in the automated version of the inspector (when no custom editor).
    /// Contains code from: 
    ///     http://answers.unity3d.com/questions/60959/mask-field-in-the-editor.html
    /// </summary>
    /// <param name="label">The string to display to the left of the control</param>
    /// <param name="selected">The LayerMask variable</param>
    /// <param name="showSpecial">True to display "Nothing" & "Everything" options</param>
    /// <returns>A new LayerMask value</returns>
    public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    {
        string selectedLayers = "";
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (layerName == "") continue;  // Skip empty layers

            if (selected == (selected | (1 << i)))
            {
                if (selectedLayers == "")
                {
                    selectedLayers = layerName;
                }
                else
                {
                    selectedLayers = "Mixed";
                    break;
                }
            }
        }

        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();
        if (Event.current.type != EventType.MouseDown &&
            Event.current.type != EventType.ExecuteCommand)
        {
            if (selected.value == 0)
                layers.Add("Nothing");
            else if (selected.value == -1)
                layers.Add("Everything");
            else
                layers.Add(selectedLayers);

            layerNumbers.Add(-1);
        }

        string check = "[x] ";
        string noCheck = "     ";
        if (showSpecial)
        {
            layers.Add((selected.value == 0 ? check : noCheck) + "Nothing");
            layerNumbers.Add(-2);

            layers.Add((selected.value == -1 ? check : noCheck) + "Everything");
            layerNumbers.Add(-3);
        }

        // A LayerMask is based on a 32bit field, so there are 32 'slots' max available
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (layerName != "")
            {
                // Add a check box to the left of any selected layer's names
                if (selected == (selected | (1 << i)))
                    layers.Add(check + layerName);
                else
                    layers.Add(noCheck + layerName);

                layerNumbers.Add(i);
            }
        }

        bool preChange = GUI.changed;
        GUI.changed = false;

        int newSelected = 0;
        if (Event.current.type == EventType.MouseDown) newSelected = -1;

        newSelected = EditorGUILayout.Popup(label,
                                            newSelected,
                                            layers.ToArray(),
                                            EditorStyles.layerMaskField);

        if (GUI.changed && newSelected >= 0)
        {
            if (showSpecial && newSelected == 0)
                selected = 0;
            else if (showSpecial && newSelected == 1)
                selected = -1;
            else
            {
                if (selected == (selected | (1 << layerNumbers[newSelected])))
                    selected &= ~(1 << layerNumbers[newSelected]);
                else
                    selected = selected | (1 << layerNumbers[newSelected]);
            }
        }
        else
        {
            GUI.changed = preChange;
        }

        return selected;
    }
    #endregion Layout Utilities



    #region Foldout Fields and Utilities
    /// <summary>
    /// Adds a fold-out list GUI from a generic list of any serialized object type
    /// </summary>
    /// <param name="list">A generic List</param>
    /// <param name="expanded">A bool to determine the state of the primary fold-out</param>
    public static bool FoldOutTextList(string label, List<string> list, bool expanded)
    {
        // Store the previous indent and return the flow to it at the end
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // A copy of toolbarButton with left alignment for foldouts
        var foldoutStyle = new GUIStyle(EditorStyles.toolbarButton);
        foldoutStyle.alignment = TextAnchor.MiddleLeft;

        expanded = AddFoldOutListHeader<string>(label, list, expanded, indent);

        // START. Will consist of one row with two columns. 
        //        The second column has the content
        EditorGUILayout.BeginHorizontal();

        // SPACER COLUMN / INDENT
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth((indent + 3) * 9));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        // CONTENT COLUMN...
        EditorGUILayout.BeginVertical();

        // Use a for, instead of foreach, to avoid the iterator since we will be
        //   be changing the loop in place when buttons are pressed. Even legal
        //   changes can throw an error when changes are detected
        for (int i = 0; i < list.Count; i++)
        {
            string item = list[i];

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // FIELD...
            if (item == null) item = "";
            list[i] = EditorGUILayout.TextField(item);

            LIST_BUTTONS listButtonPressed = AddFoldOutListItemButtons();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
            UpdateFoldOutListOnButtonPressed<string>(list, i, listButtonPressed);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel = indent;

        return expanded;
    }

    /// <summary>
    /// Adds a fold-out list GUI from a generic list of any serialized object type
    /// </summary>
    /// <param name="list">A generic List</param>
    /// <param name="expanded">A bool to determine the state of the primary fold-out</param>
    public static bool FoldOutObjList<T>(string label, List<T> list, bool expanded) where T : UnityEngine.Object
    {
        // Store the previous indent and return the flow to it at the end
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;  // Space will handle this for the header

        // A copy of toolbarButton with left alignment for foldouts
        var foldoutStyle = new GUIStyle(EditorStyles.toolbarButton);
        foldoutStyle.alignment = TextAnchor.MiddleLeft;

        if (!AddFoldOutListHeader<T>(label, list, expanded, indent))
            return false;


        // Use a for, instead of foreach, to avoid the iterator since we will be
        //   be changing the loop in place when buttons are pressed. Even legal
        //   changes can throw an error when changes are detected
        for (int i = 0; i < list.Count; i++)
        {
            T item = list[i];

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space((indent + 3) * 6); // Matches the content indent

            // OBJECT FIELD...
            // Count is always in sync bec
            T fieldVal = (T)EditorGUILayout.ObjectField(item, typeof(T), true);


            // This is weird but have to replace the item with the new value, can't
            //   find a way to set in-place in a more stable way
            list.RemoveAt(i);
            list.Insert(i, fieldVal);

            LIST_BUTTONS listButtonPressed = AddFoldOutListItemButtons();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);


            #region Process List Changes

            // Don't allow 'up' presses for the first list item
            switch (listButtonPressed)
            {
                case LIST_BUTTONS.None: // Nothing was pressed, do nothing
                    break; 

                case LIST_BUTTONS.Up:
                    if (i > 0)
                    {
                        T shiftItem = list[i];
                        list.RemoveAt(i);
                        list.Insert(i - 1, shiftItem);
                    }
                    break;

                case LIST_BUTTONS.Down:
                    // Don't allow 'down' presses for the last list item
                    if (i + 1 < list.Count)
                    {
                        T shiftItem = list[i];
                        list.RemoveAt(i);
                        list.Insert(i + 1, shiftItem);
                    }
                    break;

                case LIST_BUTTONS.Remove:
                    list.RemoveAt(i);
                    break;

                case LIST_BUTTONS.Add:
                    list.Insert(i, null);
                    break;
            }
            #endregion Process List Changes

        }

        EditorGUI.indentLevel = indent;

        return true;
    }

    /// <summary>
    /// Adds a fold-out list GUI from a generic list of any serialized object type.
    /// Uses System.Reflection to add all fields for a passed serialized object
    /// instance. Handles most basic types including automatic naming like the 
    /// inspector does by default
    /// </summary>
    /// <param name="label"> The field label</param>
    /// <param name="list">A generic List</param>
    /// <param name="expanded">A bool to determine the state of the primary fold-out</param>
    /// <param name="foldOutStates">Dictionary<object, bool> used to track list item states</param>
    /// <returns>The new foldout state from user input. Just like Unity's foldout</returns>
    public static bool FoldOutSerializedObjList<T>(string label, 
                                                   List<T> list, 
                                                   bool expanded,
                                                   ref Dictionary<object, bool> foldOutStates) 
                                          where T : new()
    {
        return SerializedObjFoldOutList<T>(label, list, expanded, ref foldOutStates, false);
    }

    /// <summary>
    /// Adds a fold-out list GUI from a generic list of any serialized object type.
    /// Uses System.Reflection to add all fields for a passed serialized object
    /// instance. Handles most basic types including automatic naming like the 
    /// inspector does by default
    /// 
    /// Adds collapseBools (see docs below)
    /// </summary>
    /// <param name="label"> The field label</param>
    /// <param name="list">A generic List</param>
    /// <param name="expanded">A bool to determine the state of the primary fold-out</param>
    /// <param name="foldOutStates">Dictionary<object, bool> used to track list item states</param>
    /// <param name="collapseBools">
    /// If true, bools on list items will collapse fields which follow them
    /// </param>
    /// <returns>The new foldout state from user input. Just like Unity's foldout</returns>
    public static bool SerializedObjFoldOutList<T>(string label, 
                                                   List<T> list, 
                                                   bool expanded,
                                                   ref Dictionary<object, bool> foldOutStates,
                                                   bool collapseBools) 
                                          where T : new()    
    {
        // Store the previous indent and return the flow to it at the end
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        int buttonSpacer = 6;

        #region Header Foldout
        // Use a Horizanal space or the toolbar will extend to the left no matter what
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel = 0;  // Space will handle this for the header
        GUILayout.Space(indent * 6); // Matches the content indent

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        expanded = Foldout(expanded, label);
        if (!expanded)
        {
            // Don't add the '+' button when the contents are collapsed. Just quit.
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = indent;  // Return to the last indent
            return expanded;
        }

        // BUTTONS...
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

        // Add expand/collapse buttons if there are items in the list
        bool masterCollapse = false;
        bool masterExpand = false;
        if (list.Count > 0)
        {
            GUIContent content;
            var collapseIcon = '\u2261'.ToString();
            content = new GUIContent(collapseIcon, "Click to collapse all");
            masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton);

            var expandIcon = '\u25A1'.ToString();
            content = new GUIContent(expandIcon, "Click to expand all");
            masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton);
        }
        else
        {
            GUILayout.FlexibleSpace();
        }

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
        // A little space between button groups
        GUILayout.Space(buttonSpacer);

        // Main Add button
        if (GUILayout.Button(new GUIContent("+", "Click to add"), EditorStyles.toolbarButton))
            list.Add(new T());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
        #endregion Header Foldout


        #region List Items
        // Use a for, instead of foreach, to avoid the iterator since we will be
        //   be changing the loop in place when buttons are pressed. Even legal
        //   changes can throw an error when changes are detected
        for (int i = 0; i < list.Count; i++)
        {
            T item = list[i];

            #region Section Header
            // If there is a field with the name 'name' use it for our label
            string itemLabel = PGEditorUtils.GetSerializedObjFieldName<T>(item);
            if (itemLabel == "") itemLabel = string.Format("Element {0}", i);


            // Get the foldout state. 
            //   If this item is new, add it too (singleton)
            //   Singleton works better than multiple Add() calls because we can do 
            //   it all at once, and in one place.
            bool foldOutState;
            if (!foldOutStates.TryGetValue(item, out foldOutState))
            {
                foldOutStates[item] = true;
                foldOutState = true;
            }

            // Force states if master buttons were pressed
            if (masterCollapse) foldOutState = false;
            if (masterExpand) foldOutState = true;

            // Use a Horizanal space or the toolbar will extend to the start no matter what
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel = 0;  // Space will handle this for the header
            GUILayout.Space((indent+3)*6); // Matches the content indent

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            // Display foldout with current state
            foldOutState = Foldout(foldOutState, itemLabel);
            foldOutStates[item] = foldOutState;  // Used again below

            LIST_BUTTONS listButtonPressed = AddFoldOutListItemButtons();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();

            #endregion Section Header
            

            // If folded out, display all serialized fields
            if (foldOutState == true)
            {
                EditorGUI.indentLevel = indent + 3;

                // Display Fields for the list instance
                PGEditorUtils.SerializedObjectFields<T>(item, collapseBools);
                GUILayout.Space(2);
            }



            #region Process List Changes
            // Don't allow 'up' presses for the first list item
            switch (listButtonPressed)
            {
                case LIST_BUTTONS.None: // Nothing was pressed, do nothing
                    break;

                case LIST_BUTTONS.Up:
                    if (i > 0)
                    {
                        T shiftItem = list[i];
                        list.RemoveAt(i);
                        list.Insert(i - 1, shiftItem);
                    }
                    break;

                case LIST_BUTTONS.Down:
                    // Don't allow 'down' presses for the last list item
                    if (i + 1 < list.Count)
                    {
                        T shiftItem = list[i];
                        list.RemoveAt(i);
                        list.Insert(i + 1, shiftItem);
                    }
                    break;

                case LIST_BUTTONS.Remove:
                    list.RemoveAt(i);
                    foldOutStates.Remove(item);  // Clean-up
                    break;

                case LIST_BUTTONS.Add:
                list.Insert(i, new T());
                    break;
            }
            #endregion Process List Changes

        }
        #endregion List Items


        EditorGUI.indentLevel = indent;

        return expanded;
    }


    /// <summary>
    /// Searches a serialized object for a field matching "name" (not case-sensitve),
    /// and if found, returns the value
    /// </summary>
    /// <param name="instance">
    /// An instance of the given type. Must be System.Serializable.
    /// </param>
    /// <returns>The name field's value or ""</returns>
    public static string GetSerializedObjFieldName<T>(T instance)
    {
        // get all public properties of T to see if there is one called 'name'
        System.Reflection.FieldInfo[] fields = typeof(T).GetFields();

        // If there is a field with the name 'name' return its value
        foreach (System.Reflection.FieldInfo fieldInfo in fields)
            if (fieldInfo.Name.ToLower() == "name")
                return ((string)fieldInfo.GetValue(instance)).DeCamel();

        // If a field type is a UnityEngine object, return its name
        //   This is done in a second loop because the first is fast as is
        foreach (System.Reflection.FieldInfo fieldInfo in fields)
        {
            try
            {
                var val = (UnityEngine.Object)fieldInfo.GetValue(instance);
                return val.name.DeCamel();
            }
            catch { }
        }

        return "";
    }

    /// <summary>
    /// Uses System.Reflection to add all fields for a passed serialized object
    /// instance. Handles most basic types including automatic naming like the 
    /// inspector does by default
    /// 
    /// Optionally, this will make a bool switch collapse the following members if they 
    /// share the first 4 characters in their name or are not a bool (will collapse from
    /// bool until it finds another bool that doesn't share the first 4 characters)
    /// </summary>
    /// <param name="instance">
    /// An instance of the given type. Must be System.Serializable.
    /// </param>
    public static void SerializedObjectFields<T>(T instance)
    {
        SerializedObjectFields<T>(instance, false);
    }

    public static void SerializedObjectFields<T>(T instance, bool collapseBools)
    {
        // get all public properties of T to see if there is one called 'name'
        System.Reflection.FieldInfo[] fields = typeof(T).GetFields();

        bool boolCollapseState = false;  // False until bool is found
        string boolCollapseName = "";    // The name of the last bool member
        string currentMemberName = "";   // The name of the member being processed

        // Display Fields Dynamically
        foreach (System.Reflection.FieldInfo fieldInfo in fields)
        {
            if (!collapseBools)
            {
                FieldInfoField<T>(instance, fieldInfo);
                continue;
            }

            // USING collapseBools...
            currentMemberName = fieldInfo.Name;

            // If this is a bool. Add the field and set collapse to true until  
            //   the end or until another bool is hit
            if (fieldInfo.FieldType == typeof(bool))
            {
                // If the first 4 letters of this bool match the last one, include this
                //   in the collapse group, rather than starting a new one.
                if (boolCollapseName.Length > 4 &&
                    currentMemberName.StartsWith(boolCollapseName.Substring(0, 4)))
                {
                    if (!boolCollapseState) FieldInfoField<T>(instance, fieldInfo);
                    continue;
                }

                FieldInfoField<T>(instance, fieldInfo);


                boolCollapseName = currentMemberName;
                boolCollapseState = !(bool)fieldInfo.GetValue(instance);
            }
            else
            {
                // Add the field unless collapse is true
                if (!boolCollapseState) FieldInfoField<T>(instance, fieldInfo);
            }

        }
    }

    /// <summary>
    /// Uses a System.Reflection.FieldInfo to add a field
    /// Handles most built-in types and components
    /// includes automatic naming like the inspector does 
    /// by default
    /// </summary>
    /// <param name="fieldInfo"></param>
    public static void FieldInfoField<T>(T instance, System.Reflection.FieldInfo fieldInfo)
    {
        string label = fieldInfo.Name.DeCamel();

        #region Built-in Data Types
        if (fieldInfo.FieldType == typeof(string))
        {
            var val = (string)fieldInfo.GetValue(instance);
            val = EditorGUILayout.TextField(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(int))
        {
            var val = (int)fieldInfo.GetValue(instance);
            val = EditorGUILayout.IntField(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            var val = (float)fieldInfo.GetValue(instance);
            val = EditorGUILayout.FloatField(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(bool))
        {
            var val = (bool)fieldInfo.GetValue(instance);
            val = EditorGUILayout.Toggle(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        #endregion Built-in Data Types

        #region Basic Unity Types
        else if (fieldInfo.FieldType == typeof(GameObject))
        {
            var val = (GameObject)fieldInfo.GetValue(instance);
            val = ObjectField<GameObject>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(Transform))
        {
            var val = (Transform)fieldInfo.GetValue(instance);
            val = ObjectField<Transform>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(Rigidbody))
        {
            var val = (Rigidbody)fieldInfo.GetValue(instance);
            val = ObjectField<Rigidbody>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(Renderer))
        {
            var val = (Renderer)fieldInfo.GetValue(instance);
            val = ObjectField<Renderer>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(Mesh))
        {
            var val = (Mesh)fieldInfo.GetValue(instance);
            val = ObjectField<Mesh>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        #endregion Basic Unity Types

        #region Unity Collider Types
        else if (fieldInfo.FieldType == typeof(BoxCollider))
        {
            var val = (BoxCollider)fieldInfo.GetValue(instance);
            val = ObjectField<BoxCollider>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(SphereCollider))
        {
            var val = (SphereCollider)fieldInfo.GetValue(instance);
            val = ObjectField<SphereCollider>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(CapsuleCollider))
        {
            var val = (CapsuleCollider)fieldInfo.GetValue(instance);
            val = ObjectField<CapsuleCollider>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(MeshCollider))
        {
            var val = (MeshCollider)fieldInfo.GetValue(instance);
            val = ObjectField<MeshCollider>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        else if (fieldInfo.FieldType == typeof(WheelCollider))
        {
            var val = (WheelCollider)fieldInfo.GetValue(instance);
            val = ObjectField<WheelCollider>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        #endregion Unity Collider Types

        #region Other Unity Types
        else if (fieldInfo.FieldType == typeof(CharacterController))
        {
            var val = (CharacterController)fieldInfo.GetValue(instance);
            val = ObjectField<CharacterController>(label, val);
            fieldInfo.SetValue(instance, val);
            return;
        }
        #endregion Other Unity Types
    }


    /// <summary>
    /// Adds the GUI header line which contains the label and add buttons.
    /// </summary>
    /// <param name="label">The visible label in the GUI</param>
    /// <param name="list">Needed to add a new item if count is 0</param>
    /// <param name="expanded"></param>
    /// <param name="lastIndent"></param>
    private static bool AddFoldOutListHeader<T>(string label, List<T> list, bool expanded, int lastIndent)
    {
        int buttonSpacer = 6;

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        expanded = PGEditorUtils.Foldout(expanded, label);
        if (!expanded)
        {
            // Don't add the '+' button when the contents are collapsed. Just quit.
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = lastIndent;  // Return to the last indent
            return expanded;
        }

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));   // 1/2 the item button width
        GUILayout.Space(buttonSpacer);

        // Master add at end button. List items will insert
        if (GUILayout.Button(new GUIContent("+", "Click to add"), EditorStyles.toolbarButton))
            list.Add(default(T));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        return expanded;
    }

    /// <summary>
    /// Used by AddFoldOutListItemButtons to return which button was pressed, and by 
    /// UpdateFoldOutListOnButtonPressed to process the pressed button for regular lists
    /// </summary>
    protected enum LIST_BUTTONS { None, Up, Down, Add, Remove }

    /// <summary>
    /// Adds the buttons which control a list item
    /// </summary>
    /// <returns>LIST_BUTTONS - The LIST_BUTTONS pressed or LIST_BUTTONS.None</returns>
    private static LIST_BUTTONS AddFoldOutListItemButtons()
    {
        #region Layout
        int buttonSpacer = 6;

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
        // The up arrow will move things towards the beginning of the List
        var upArrow = '\u25B2'.ToString();
        bool upPressed = GUILayout.Button(new GUIContent(upArrow, "Click to shift up"),
                                          EditorStyles.toolbarButton);

        // The down arrow will move things towards the end of the List
        var dnArrow = '\u25BC'.ToString();
        bool downPressed = GUILayout.Button(new GUIContent(dnArrow, "Click to shift down"),
                                            EditorStyles.toolbarButton);

        // A little space between button groups
        GUILayout.Space(buttonSpacer);

        // Remove Button - Process presses later
        bool removePressed = GUILayout.Button(new GUIContent("-", "Click to remove"),
                                              EditorStyles.toolbarButton);

        // Add button - Process presses later
        bool addPressed = GUILayout.Button(new GUIContent("+", "Click to insert new"),
                                           EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();
        #endregion Layout

        // Return the pressed button if any
        if (upPressed == true) return LIST_BUTTONS.Up;
        if (downPressed == true) return LIST_BUTTONS.Down;
        if (removePressed == true) return LIST_BUTTONS.Remove;
        if (addPressed == true) return LIST_BUTTONS.Add;

        return LIST_BUTTONS.None;
    }

    /// <summary>
    /// Used by basic foldout lists to process any list item button presses which will alter
    /// the order or members of the ist
    /// </summary>
    /// <param name="listButtonPressed"></param>
    private static void UpdateFoldOutListOnButtonPressed<T>(List<T> list, int currentIndex, LIST_BUTTONS listButtonPressed)
    {
        // Don't allow 'up' presses for the first list item
        switch (listButtonPressed)
        {
            case LIST_BUTTONS.None: // Nothing was pressed, do nothing
                break;

            case LIST_BUTTONS.Up:
                if (currentIndex > 0)
                {
                    T shiftItem = list[currentIndex];
                    list.RemoveAt(currentIndex);
                    list.Insert(currentIndex - 1, shiftItem);
                }
                break;

            case LIST_BUTTONS.Down:
                // Don't allow 'down' presses for the last list item
                if (currentIndex + 1 < list.Count)
                {
                    T shiftItem = list[currentIndex];
                    list.RemoveAt(currentIndex);
                    list.Insert(currentIndex + 1, shiftItem);
                }
                break;

            case LIST_BUTTONS.Remove:
                list.RemoveAt(currentIndex);
                break;

            case LIST_BUTTONS.Add:
                list.Insert(currentIndex, default(T));
                break;
        }
    }



    /// <summary>
    /// Adds a foldout in 'LookLikeInspector' which has a full bar to click on, not just
    /// the little triangle. It also adds a default tool-tip.
    /// </summary>
    /// <param name="expanded"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    public static bool Foldout(bool expanded, string label)
    {
#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		EditorGUIUtility.LookLikeInspector();
#endif
        var content = new GUIContent(label, FOLD_OUT_TOOL_TIP);
        expanded = EditorGUILayout.Foldout(expanded, content);
        SetLabelWidth();

        return expanded;
    }
    #endregion Foldout Fields and Utilities

}



public static class PGEditorToolsStringExtensions
{
    /// <summary>
    /// Converts a string from camel-case to seperate words that start with 
    /// capital letters. Also removes leading underscores.
    /// </summary>
    /// <returns>string</returns>
    public static string DeCamel(this string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;

        System.Text.StringBuilder newStr = new System.Text.StringBuilder();

        char c;
        for (int i = 0; i < s.Length; i++)
        {
            c = s[i];

            // Handle spaces and underscores. 
            //   Do not keep underscores
            //   Only keep spaces if there is a lower case letter next, and 
            //       capitalize the letter
            if (c == ' ' || c == '_')
            { 
                // Only check the next character is there IS a next character
                if (i < s.Length-1 && char.IsLower(s[i+1]))
                {
                    // If it isn't the first character, add a space before it
                    if (newStr.Length != 0)
                    {
                        newStr.Append(' ');  // Add the space
                        newStr.Append(char.ToUpper(s[i + 1]));
                    }
                    else
                    {
                        newStr.Append(s[i + 1]);  // Stripped if first char in string
                    }

                    i++;  // Skip the next. We already used it
                }
            }  // Handle uppercase letters
            else if (char.IsUpper(c))    
            {
                // If it isn't the first character, add a space before it
                if (newStr.Length != 0)
                {
                    newStr.Append(' ');
                    newStr.Append(c);
                }
                else
                {
                    newStr.Append(c);
                }
            } 
            else  // Normal character. Store and move on.
            {
                newStr.Append(c);
            }
        }

        return newStr.ToString();
    }


    /// <summary>
    /// Capitalizes only the first letter of a string
    /// </summary>
    /// <returns>string</returns>
    public static string Capitalize(this string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

}