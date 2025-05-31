/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QFramework
{
    /// <summary> xNode-specific version of <see cref="EditorGUILayout"/> </summary>
    public static class GUIGraphGUILayout
    {
        private static readonly Dictionary<UnityEngine.Object, Dictionary<string, ReorderableList>>
            reorderableListCache = new Dictionary<UnityEngine.Object, Dictionary<string, ReorderableList>>();

        private static int reorderableListIndex = -1;

        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        public static void PropertyField(SerializedProperty property, bool includeChildren = true,
            params GUILayoutOption[] options)
        {
            PropertyField(property, (GUIContent)null, includeChildren, options);
        }

        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        public static void PropertyField(SerializedProperty property, GUIContent label, bool includeChildren = true,
            params GUILayoutOption[] options)
        {
            if (property == null) throw new NullReferenceException();
            GUIGraphNode node = property.serializedObject.targetObject as GUIGraphNode;
            GUIGraphNodePort port = node.GetPort(property.name);
            PropertyField(property, label, port, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void PropertyField(SerializedProperty property, GUIGraphNodePort port,
            bool includeChildren = true, params GUILayoutOption[] options)
        {
            PropertyField(property, null, port, includeChildren, options);
        }

        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void PropertyField(SerializedProperty property, GUIContent label, GUIGraphNodePort port,
            bool includeChildren = true, params GUILayoutOption[] options)
        {
            if (property == null) throw new NullReferenceException();

            // If property is not a port, display a regular property field
            if (port == null) EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
            else
            {
                Rect rect = new Rect();

                List<PropertyAttribute> propertyAttributes =
                    GUIGraphUtilities.GetCachedPropertyAttribs(port.node.GetType(), property.name);

                // If property is an input, display a regular property field and put a port handle on the left side
                if (port.direction == GUIGraphNodePort.IO.Input)
                {
                    // Get data from [Input] attribute
                    GUIGraphNode.ShowBackingValue showBacking = GUIGraphNode.ShowBackingValue.Unconnected;
                    GUIGraphNode.InputAttribute inputAttribute;
                    bool dynamicPortList = false;
                    if (GUIGraphUtilities.GetCachedAttrib(port.node.GetType(), property.name, out inputAttribute))
                    {
                        dynamicPortList = inputAttribute.dynamicPortList;
                        showBacking = inputAttribute.backingValue;
                    }

                    bool usePropertyAttributes = dynamicPortList ||
                                                 showBacking == GUIGraphNode.ShowBackingValue.Never ||
                                                 (showBacking == GUIGraphNode.ShowBackingValue.Unconnected &&
                                                  port.IsConnected);

                    float spacePadding = 0;
                    foreach (var attr in propertyAttributes)
                    {
                        if (attr is SpaceAttribute)
                        {
                            if (usePropertyAttributes) GUILayout.Space((attr as SpaceAttribute).height);
                            else spacePadding += (attr as SpaceAttribute).height;
                        }
                        else if (attr is HeaderAttribute)
                        {
                            if (usePropertyAttributes)
                            {
                                //GUI Values are from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs
                                Rect position = GUILayoutUtility.GetRect(0,
                                    (EditorGUIUtility.singleLineHeight * 1.5f) -
                                    EditorGUIUtility
                                        .standardVerticalSpacing); //Layout adds standardVerticalSpacing after rect so we subtract it.
                                position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
                                position = EditorGUI.IndentedRect(position);
                                GUI.Label(position, (attr as HeaderAttribute).header, EditorStyles.boldLabel);
                            }
                            else spacePadding += EditorGUIUtility.singleLineHeight * 1.5f;
                        }
                    }

                    if (dynamicPortList)
                    {
                        Type type = GetType(property);
                        GUIGraphNode.ConnectionType connectionType = inputAttribute != null
                            ? inputAttribute.connectionType
                            : GUIGraphNode.ConnectionType.Multiple;
                        DynamicPortList(property.name, type, property.serializedObject, port.direction, connectionType);
                        return;
                    }

                    switch (showBacking)
                    {
                        case GUIGraphNode.ShowBackingValue.Unconnected:
                            // Display a label if port is connected
                            if (port.IsConnected)
                                EditorGUILayout.LabelField(label != null
                                    ? label
                                    : new GUIContent(property.displayName));
                            // Display an editable property field if port is not connected
                            else
                                EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                        case GUIGraphNode.ShowBackingValue.Never:
                            // Display a label
                            EditorGUILayout.LabelField(label != null ? label : new GUIContent(property.displayName));
                            break;
                        case GUIGraphNode.ShowBackingValue.Always:
                            // Display an editable property field
                            EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                    }

                    rect = GUILayoutUtility.GetLastRect();
                    rect.position = rect.position - new Vector2(16, -spacePadding);
                    // If property is an output, display a text label and put a port handle on the right side
                }
                else if (port.direction == GUIGraphNodePort.IO.Output)
                {
                    // Get data from [Output] attribute
                    GUIGraphNode.ShowBackingValue showBacking = GUIGraphNode.ShowBackingValue.Unconnected;
                    GUIGraphNode.OutputAttribute outputAttribute;
                    bool dynamicPortList = false;
                    if (GUIGraphUtilities.GetCachedAttrib(port.node.GetType(), property.name, out outputAttribute))
                    {
                        dynamicPortList = outputAttribute.dynamicPortList;
                        showBacking = outputAttribute.backingValue;
                    }

                    bool usePropertyAttributes = dynamicPortList ||
                                                 showBacking == GUIGraphNode.ShowBackingValue.Never ||
                                                 (showBacking == GUIGraphNode.ShowBackingValue.Unconnected &&
                                                  port.IsConnected);

                    float spacePadding = 0;
                    foreach (var attr in propertyAttributes)
                    {
                        if (attr is SpaceAttribute)
                        {
                            if (usePropertyAttributes) GUILayout.Space((attr as SpaceAttribute).height);
                            else spacePadding += (attr as SpaceAttribute).height;
                        }
                        else if (attr is HeaderAttribute)
                        {
                            if (usePropertyAttributes)
                            {
                                //GUI Values are from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs
                                Rect position = GUILayoutUtility.GetRect(0,
                                    (EditorGUIUtility.singleLineHeight * 1.5f) -
                                    EditorGUIUtility
                                        .standardVerticalSpacing); //Layout adds standardVerticalSpacing after rect so we subtract it.
                                position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
                                position = EditorGUI.IndentedRect(position);
                                GUI.Label(position, (attr as HeaderAttribute).header, EditorStyles.boldLabel);
                            }
                            else spacePadding += EditorGUIUtility.singleLineHeight * 1.5f;
                        }
                    }

                    if (dynamicPortList)
                    {
                        Type type = GetType(property);
                        GUIGraphNode.ConnectionType connectionType = outputAttribute != null
                            ? outputAttribute.connectionType
                            : GUIGraphNode.ConnectionType.Multiple;
                        DynamicPortList(property.name, type, property.serializedObject, port.direction, connectionType);
                        return;
                    }

                    switch (showBacking)
                    {
                        case GUIGraphNode.ShowBackingValue.Unconnected:
                            // Display a label if port is connected
                            if (port.IsConnected)
                                EditorGUILayout.LabelField(label != null ? label : new GUIContent(property.displayName),
                                    GUIGraphResources.OutputPort, GUILayout.MinWidth(30));
                            // Display an editable property field if port is not connected
                            else
                                EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                        case GUIGraphNode.ShowBackingValue.Never:
                            // Display a label
                            EditorGUILayout.LabelField(label != null ? label : new GUIContent(property.displayName),
                                GUIGraphResources.OutputPort, GUILayout.MinWidth(30));
                            break;
                        case GUIGraphNode.ShowBackingValue.Always:
                            // Display an editable property field
                            EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
                            break;
                    }

                    rect = GUILayoutUtility.GetLastRect();
                    rect.position = rect.position + new Vector2(rect.width, spacePadding);
                }

                rect.size = new Vector2(16, 16);

                GUIGraphNodeEditor editor = GUIGraphNodeEditor.GetEditor(port.node, GUIGraphWindow.current);
                Color backgroundColor = editor.GetTint();
                Color col = GUIGraphWindow.current.graphEditor.GetPortColor(port);
                DrawPortHandle(rect, backgroundColor, col);

                // Register the handle position
                Vector2 portPos = rect.center;
                GUIGraphNodeEditor.portPositions[port] = portPos;
            }
        }

        private static System.Type GetType(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetFieldInfo(property.name);
            return fi.FieldType;
        }

        /// <summary> Make a simple port field. </summary>
        public static void PortField(GUIGraphNodePort port, params GUILayoutOption[] options)
        {
            PortField(null, port, options);
        }

        /// <summary> Make a simple port field. </summary>
        public static void PortField(GUIContent label, GUIGraphNodePort port, params GUILayoutOption[] options)
        {
            if (port == null) return;
            if (options == null) options = new GUILayoutOption[] { GUILayout.MinWidth(30) };
            Vector2 position = Vector3.zero;
            GUIContent content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == GUIGraphNodePort.IO.Input)
            {
                // Display a label
                EditorGUILayout.LabelField(content, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == GUIGraphNodePort.IO.Output)
            {
                // Display a label
                EditorGUILayout.LabelField(content, GUIGraphResources.OutputPort, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }

            PortField(position, port);
        }

        /// <summary> Make a simple port field. </summary>
        public static void PortField(Vector2 position, GUIGraphNodePort port)
        {
            if (port == null) return;

            Rect rect = new Rect(position, new Vector2(16, 16));

            GUIGraphNodeEditor editor = GUIGraphNodeEditor.GetEditor(port.node, GUIGraphWindow.current);
            Color backgroundColor = editor.GetTint();
            Color col = GUIGraphWindow.current.graphEditor.GetPortColor(port);
            DrawPortHandle(rect, backgroundColor, col);

            // Register the handle position
            Vector2 portPos = rect.center;
            GUIGraphNodeEditor.portPositions[port] = portPos;
        }

        /// <summary> Add a port field to previous layout element. </summary>
        public static void AddPortField(GUIGraphNodePort port)
        {
            if (port == null) return;
            Rect rect = new Rect();

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == GUIGraphNodePort.IO.Input)
            {
                rect = GUILayoutUtility.GetLastRect();
                rect.position = rect.position - new Vector2(16, 0);
                // If property is an output, display a text label and put a port handle on the right side
            }
            else if (port.direction == GUIGraphNodePort.IO.Output)
            {
                rect = GUILayoutUtility.GetLastRect();
                rect.position = rect.position + new Vector2(rect.width, 0);
            }

            rect.size = new Vector2(16, 16);

            GUIGraphNodeEditor editor = GUIGraphNodeEditor.GetEditor(port.node, GUIGraphWindow.current);
            Color backgroundColor = editor.GetTint();
            Color col = GUIGraphWindow.current.graphEditor.GetPortColor(port);
            DrawPortHandle(rect, backgroundColor, col);

            // Register the handle position
            Vector2 portPos = rect.center;
            GUIGraphNodeEditor.portPositions[port] = portPos;
        }

        /// <summary> Draws an input and an output port on the same line </summary>
        public static void PortPair(GUIGraphNodePort input, GUIGraphNodePort output)
        {
            GUILayout.BeginHorizontal();
            GUIGraphGUILayout.PortField(input, GUILayout.MinWidth(0));
            GUIGraphGUILayout.PortField(output, GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();
        }

        public static void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, GUIGraphResources.DotOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, GUIGraphResources.Dot);
            GUI.color = col;
        }

        /// <summary> Is this port part of a DynamicPortList? </summary>
        public static bool IsDynamicPortListPort(GUIGraphNodePort port)
        {
            string[] parts = port.fieldName.Split(' ');
            if (parts.Length != 2) return false;
            Dictionary<string, ReorderableList> cache;
            if (reorderableListCache.TryGetValue(port.node, out cache))
            {
                ReorderableList list;
                if (cache.TryGetValue(parts[0], out list)) return true;
            }

            return false;
        }

        /// <summary> Draw an editable list of dynamic ports. Port names are named as "[fieldName] [index]" </summary>
        /// <param name="fieldName">Supply a list for editable values</param>
        /// <param name="type">Value type of added dynamic ports</param>
        /// <param name="serializedObject">The serializedObject of the node</param>
        /// <param name="connectionType">Connection type of added dynamic ports</param>
        /// <param name="onCreation">Called on the list on creation. Use this if you want to customize the created ReorderableList</param>
        public static void DynamicPortList(string fieldName, Type type, SerializedObject serializedObject,
            GUIGraphNodePort.IO io,
            GUIGraphNode.ConnectionType connectionType = GUIGraphNode.ConnectionType.Multiple,
            GUIGraphNode.TypeConstraint typeConstraint = GUIGraphNode.TypeConstraint.None,
            Action<ReorderableList> onCreation = null)
        {
            GUIGraphNode node = serializedObject.targetObject as GUIGraphNode;

            var indexedPorts = node.DynamicPorts.Select(x =>
            {
                string[] split = x.fieldName.Split(' ');
                if (split != null && split.Length == 2 && split[0] == fieldName)
                {
                    int i = -1;
                    if (int.TryParse(split[1], out i))
                    {
                        return new { index = i, port = x };
                    }
                }

                return new { index = -1, port = (GUIGraphNodePort)null };
            }).Where(x => x.port != null);
            List<GUIGraphNodePort> dynamicPorts = indexedPorts.OrderBy(x => x.index).Select(x => x.port).ToList();

            node.UpdatePorts();

            ReorderableList list = null;
            Dictionary<string, ReorderableList> rlc;
            if (reorderableListCache.TryGetValue(serializedObject.targetObject, out rlc))
            {
                if (!rlc.TryGetValue(fieldName, out list)) list = null;
            }

            // If a ReorderableList isn't cached for this array, do so.
            if (list == null)
            {
                SerializedProperty arrayData = serializedObject.FindProperty(fieldName);
                list = CreateReorderableList(fieldName, dynamicPorts, arrayData, type, serializedObject, io,
                    connectionType, typeConstraint, onCreation);
                if (reorderableListCache.TryGetValue(serializedObject.targetObject, out rlc)) rlc.Add(fieldName, list);
                else
                    reorderableListCache.Add(serializedObject.targetObject,
                        new Dictionary<string, ReorderableList>() { { fieldName, list } });
            }

            list.list = dynamicPorts;
            list.DoLayoutList();
        }

        private static ReorderableList CreateReorderableList(string fieldName, List<GUIGraphNodePort> dynamicPorts,
            SerializedProperty arrayData, Type type, SerializedObject serializedObject, GUIGraphNodePort.IO io,
            GUIGraphNode.ConnectionType connectionType, GUIGraphNode.TypeConstraint typeConstraint,
            Action<ReorderableList> onCreation)
        {
            bool hasArrayData = arrayData != null && arrayData.isArray;
            GUIGraphNode node = serializedObject.targetObject as GUIGraphNode;
            ReorderableList list = new ReorderableList(dynamicPorts, null, true, true, true, true);
            string label = arrayData != null ? arrayData.displayName : ObjectNames.NicifyVariableName(fieldName);

            list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    GUIGraphNodePort port = node.GetPort(fieldName + " " + index);
                    if (hasArrayData && arrayData.propertyType != SerializedPropertyType.String)
                    {
                        if (arrayData.arraySize <= index)
                        {
                            EditorGUI.LabelField(rect, "Array[" + index + "] data out of range");
                            return;
                        }

                        SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(rect, itemData, true);
                    }
                    else EditorGUI.LabelField(rect, port != null ? port.fieldName : "");

                    if (port != null)
                    {
                        Vector2 pos = rect.position +
                                      (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
                        GUIGraphGUILayout.PortField(pos, port);
                    }
                };
            list.elementHeightCallback =
                (int index) =>
                {
                    if (hasArrayData)
                    {
                        if (arrayData.arraySize <= index) return EditorGUIUtility.singleLineHeight;
                        SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);
                        return EditorGUI.GetPropertyHeight(itemData);
                    }
                    else return EditorGUIUtility.singleLineHeight;
                };
            list.drawHeaderCallback =
                (Rect rect) => { EditorGUI.LabelField(rect, label); };
            list.onSelectCallback =
                (ReorderableList rl) => { reorderableListIndex = rl.index; };
            list.onReorderCallback =
                (ReorderableList rl) =>
                {
                    bool hasRect = false;
                    bool hasNewRect = false;
                    Rect rect = Rect.zero;
                    Rect newRect = Rect.zero;
                    // Move up
                    if (rl.index > reorderableListIndex)
                    {
                        for (int i = reorderableListIndex; i < rl.index; ++i)
                        {
                            GUIGraphNodePort port = node.GetPort(fieldName + " " + i);
                            GUIGraphNodePort nextPort = node.GetPort(fieldName + " " + (i + 1));
                            port.SwapConnections(nextPort);

                            // Swap cached positions to mitigate twitching
                            hasRect = GUIGraphWindow.current.portConnectionPoints.TryGetValue(port, out rect);
                            hasNewRect =
                                GUIGraphWindow.current.portConnectionPoints.TryGetValue(nextPort, out newRect);
                            GUIGraphWindow.current.portConnectionPoints[port] = hasNewRect ? newRect : rect;
                            GUIGraphWindow.current.portConnectionPoints[nextPort] = hasRect ? rect : newRect;
                        }
                    }
                    // Move down
                    else
                    {
                        for (int i = reorderableListIndex; i > rl.index; --i)
                        {
                            GUIGraphNodePort port = node.GetPort(fieldName + " " + i);
                            GUIGraphNodePort nextPort = node.GetPort(fieldName + " " + (i - 1));
                            port.SwapConnections(nextPort);

                            // Swap cached positions to mitigate twitching
                            hasRect = GUIGraphWindow.current.portConnectionPoints.TryGetValue(port, out rect);
                            hasNewRect =
                                GUIGraphWindow.current.portConnectionPoints.TryGetValue(nextPort, out newRect);
                            GUIGraphWindow.current.portConnectionPoints[port] = hasNewRect ? newRect : rect;
                            GUIGraphWindow.current.portConnectionPoints[nextPort] = hasRect ? rect : newRect;
                        }
                    }

                    // Apply changes
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();

                    // Move array data if there is any
                    if (hasArrayData)
                    {
                        arrayData.MoveArrayElement(reorderableListIndex, rl.index);
                    }

                    // Apply changes
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    GUIGraphWindow.current.Repaint();
                    EditorApplication.delayCall += GUIGraphWindow.current.Repaint;
                };
            list.onAddCallback =
                (ReorderableList rl) =>
                {
                    // Add dynamic port postfixed with an index number
                    string newName = fieldName + " 0";
                    int i = 0;
                    while (node.HasPort(newName)) newName = fieldName + " " + (++i);

                    if (io == GUIGraphNodePort.IO.Output)
                        node.AddDynamicOutput(type, connectionType, GUIGraphNode.TypeConstraint.None, newName);
                    else node.AddDynamicInput(type, connectionType, typeConstraint, newName);
                    serializedObject.Update();
                    EditorUtility.SetDirty(node);
                    if (hasArrayData)
                    {
                        arrayData.InsertArrayElementAtIndex(arrayData.arraySize);
                    }

                    serializedObject.ApplyModifiedProperties();
                };
            list.onRemoveCallback =
                (ReorderableList rl) =>
                {
                    var indexedPorts = node.DynamicPorts.Select(x =>
                    {
                        string[] split = x.fieldName.Split(' ');
                        if (split != null && split.Length == 2 && split[0] == fieldName)
                        {
                            int i = -1;
                            if (int.TryParse(split[1], out i))
                            {
                                return new { index = i, port = x };
                            }
                        }

                        return new { index = -1, port = (GUIGraphNodePort)null };
                    }).Where(x => x.port != null);
                    dynamicPorts = indexedPorts.OrderBy(x => x.index).Select(x => x.port).ToList();

                    int index = rl.index;

                    if (dynamicPorts[index] == null)
                    {
                        Debug.LogWarning("No port found at index " + index + " - Skipped");
                    }
                    else if (dynamicPorts.Count <= index)
                    {
                        Debug.LogWarning("DynamicPorts[" + index + "] out of range. Length was " + dynamicPorts.Count +
                                         " - Skipped");
                    }
                    else
                    {
                        // Clear the removed ports connections
                        dynamicPorts[index].ClearConnections();
                        // Move following connections one step up to replace the missing connection
                        for (int k = index + 1; k < dynamicPorts.Count(); k++)
                        {
                            for (int j = 0; j < dynamicPorts[k].ConnectionCount; j++)
                            {
                                GUIGraphNodePort other = dynamicPorts[k].GetConnection(j);
                                dynamicPorts[k].Disconnect(other);
                                dynamicPorts[k - 1].Connect(other);
                            }
                        }

                        // Remove the last dynamic port, to avoid messing up the indexing
                        node.RemoveDynamicPort(dynamicPorts[dynamicPorts.Count() - 1].fieldName);
                        serializedObject.Update();
                        EditorUtility.SetDirty(node);
                    }

                    if (hasArrayData && arrayData.propertyType != SerializedPropertyType.String)
                    {
                        if (arrayData.arraySize <= index)
                        {
                            Debug.LogWarning("Attempted to remove array index " + index + " where only " +
                                             arrayData.arraySize + " exist - Skipped");
                            Debug.Log(rl.list[0]);
                            return;
                        }

                        arrayData.DeleteArrayElementAtIndex(index);
                        // Error handling. If the following happens too often, file a bug report at https://github.com/Siccity/xNode/issues
                        if (dynamicPorts.Count <= arrayData.arraySize)
                        {
                            while (dynamicPorts.Count <= arrayData.arraySize)
                            {
                                arrayData.DeleteArrayElementAtIndex(arrayData.arraySize - 1);
                            }

                            UnityEngine.Debug.LogWarning(
                                "Array size exceeded dynamic ports size. Excess items removed.");
                        }

                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }
                };

            if (hasArrayData)
            {
                int dynamicPortCount = dynamicPorts.Count;
                while (dynamicPortCount < arrayData.arraySize)
                {
                    // Add dynamic port postfixed with an index number
                    string newName = arrayData.name + " 0";
                    int i = 0;
                    while (node.HasPort(newName)) newName = arrayData.name + " " + (++i);
                    if (io == GUIGraphNodePort.IO.Output)
                        node.AddDynamicOutput(type, connectionType, typeConstraint, newName);
                    else node.AddDynamicInput(type, connectionType, typeConstraint, newName);
                    EditorUtility.SetDirty(node);
                    dynamicPortCount++;
                }

                while (arrayData.arraySize < dynamicPortCount)
                {
                    arrayData.InsertArrayElementAtIndex(arrayData.arraySize);
                }

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            if (onCreation != null) onCreation(list);
            return list;
        }
    }
}
#endif