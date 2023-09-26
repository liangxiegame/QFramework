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
using QFramework.Internal;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
    [CustomNodeEditor(typeof(GUIGraphNode))]
    public class GUIGraphNodeEditor : GUIGraphEditorBase<GUIGraphNodeEditor,
        GUIGraphNodeEditor.CustomNodeEditorAttribute, GUIGraphNode>
    {
        private readonly Color DEFAULTCOLOR = new Color32(57, 58, 64, byte.MaxValue);

        /// <summary> Fires every whenever a node was modified through the editor </summary>
        public static Action<GUIGraphNode> onUpdateNode;

        public readonly static Dictionary<GUIGraphNodePort, Vector2> portPositions =
            new Dictionary<GUIGraphNodePort, Vector2>();

        public virtual void OnHeaderGUI()
        {
            GUILayout.Label(target.name, GUIGraphResources.styles.NodeHeader, GUILayout.Height(30));
        }

        /// <summary> Draws standard field editors for all public fields </summary>
        public virtual void OnBodyGUI()
        {
            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };


            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                GUIGraphGUILayout.PropertyField(iterator, true);
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (GUIGraphNodePort dynamicPort in target.DynamicPorts)
            {
                if (GUIGraphGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                GUIGraphGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual int GetWidth()
        {
            Type type = target.GetType();
            int width;
            if (type.TryGetAttributeWidth(out width)) return width;
            else return 208;
        }

        /// <summary> Returns color for target node </summary>
        public virtual Color GetTint()
        {
            // Try get color from [NodeTint] attribute
            Type type = target.GetType();
            Color color;
            if (type.TryGetAttributeTint(out color)) return color;
            // Return default color (grey)
            else return DEFAULTCOLOR;
        }

        public virtual GUIStyle GetBodyStyle()
        {
            return GUIGraphResources.styles.nodeBody;
        }

        public virtual GUIStyle GetBodyHighlightStyle()
        {
            return GUIGraphResources.styles.NodeHighlight;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu)
        {
            bool canRemove = true;
            // Actions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is GUIGraphNode)
            {
                GUIGraphNode node = Selection.activeObject as GUIGraphNode;
                menu.AddItem(new GUIContent("Move To Top"), false,
                    () => GUIGraphWindow.current.MoveNodeToTop(node));
                menu.AddItem(new GUIContent("Rename"), false, GUIGraphWindow.current.RenameSelectedNode);

                canRemove = GUIGraphEditor.GetEditor(node.graph, GUIGraphWindow.current).CanRemove(node);
            }

            // Add actions to any number of selected nodes
            menu.AddItem(new GUIContent("Copy"), false, GUIGraphWindow.current.CopySelectedNodes);
            menu.AddItem(new GUIContent("Duplicate"), false, GUIGraphWindow.current.DuplicateSelectedNodes);

            if (canRemove)
                menu.AddItem(new GUIContent("Remove"), false, GUIGraphWindow.current.RemoveSelectedNodes);
            else menu.AddItem(new GUIContent("Remove"), false, null);

            // Custom sctions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is GUIGraphNode)
            {
                GUIGraphNode node = Selection.activeObject as GUIGraphNode;
                menu.AddCustomContextMenuItems(node);
            }
        }

        /// <summary> Rename the node asset. This will trigger a reimport of the node. </summary>
        public void Rename(string newName)
        {
            if (newName == null || newName.Trim() == "")
                newName = GUIGraphUtilities.NodeDefaultName(target.GetType());
            target.name = newName;
            OnRename();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
        }

        /// <summary> Called after this node's name has changed. </summary>
        public virtual void OnRename()
        {
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeEditorAttribute : Attribute,
            GUIGraphEditorBase<GUIGraphNodeEditor, GUIGraphNodeEditor.CustomNodeEditorAttribute,
                GUIGraphNode>.INodeEditorAttrib
        {
            private Type inspectedType;

            /// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            public CustomNodeEditorAttribute(Type inspectedType)
            {
                this.inspectedType = inspectedType;
            }

            public Type GetInspectedType()
            {
                return inspectedType;
            }
        }
    }
}
#endif