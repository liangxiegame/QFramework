/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Linq;
using QFramework.Internal;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    /// <summary> Base class to derive custom Node Graph editors from. Use this to override how graphs are drawn in the editor. </summary>
    [CustomNodeGraphEditor(typeof(GUIGraph))]
    public class GUIGraphEditor : GUIGraphEditorBase<GUIGraphEditor, GUIGraphEditor.CustomNodeGraphEditorAttribute, GUIGraph>
    {

        /// <summary> Are we currently renaming a node? </summary>
        protected bool isRenaming;

        public virtual void OnGUI()
        {
        }

        /// <summary> Called when opened by NodeEditorWindow </summary>
        public virtual void OnOpen()
        {
        }

        /// <summary> Called when NodeEditorWindow gains focus </summary>
        public virtual void OnWindowFocus()
        {
        }

        /// <summary> Called when NodeEditorWindow loses focus </summary>
        public virtual void OnWindowFocusLost()
        {
        }


        public virtual void OnLanguageChanged()
        {
            
        }
        
        public virtual Texture2D GetGridTexture()
        {
            return GUIGraphPreferences.GetSettings().gridTexture;
        }

        public virtual Texture2D GetSecondaryGridTexture()
        {
            return GUIGraphPreferences.GetSettings().crossTexture;
        }

        /// <summary> Return default settings for this graph type. This is the settings the user will load if no previous settings have been saved. </summary>
        public virtual GUIGraphPreferences.Settings GetDefaultPreferences()
        {
            return new GUIGraphPreferences.Settings();
        }

        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
        public virtual string GetNodeMenuName(Type type)
        {
            //Check if type has the CreateNodeMenuAttribute
            GUIGraphNode.CreateNodeMenuAttribute attrib;
            if (GUIGraphUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.menuName;
            else // Return generated path
                return GUIGraphUtilities.NodeDefaultPath(type);
        }

        /// <summary> The order by which the menu items are displayed. </summary>
        public virtual int GetNodeMenuOrder(Type type)
        {
            //Check if type has the CreateNodeMenuAttribute
            GUIGraphNode.CreateNodeMenuAttribute attrib;
            if (GUIGraphUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.order;
            else
                return 0;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu)
        {
            Vector2 pos = GUIGraphWindow.current.WindowToGridPosition(Event.current.mousePosition);
            var nodeTypes = GUIGraphReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            for (int i = 0; i < nodeTypes.Length; i++)
            {
                Type type = nodeTypes[i];

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type
                GUIGraphNode.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (GUIGraphUtilities.GetAttrib(type, out disallowAttrib))
                {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                // Add node entry to context menu
                if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                else
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        GUIGraphNode node = CreateNode(type, pos);
                        GUIGraphWindow.current.AutoConnect(node);
                    });
            }

            menu.AddSeparator("");
            if (GUIGraphWindow.copyBuffer != null && GUIGraphWindow.copyBuffer.Length > 0)
                menu.AddItem(new GUIContent("Paste"), false, () => GUIGraphWindow.current.PasteNodes(pos));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            // menu.AddItem(new GUIContent("Preferences"), false, () => IMGUIGraphReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }

        /// <summary> Returned gradient is used to color noodles </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual Gradient GetNoodleGradient(GUIGraphNodePort output, GUIGraphNodePort input)
        {
            Gradient grad = new Gradient();

            // If dragging the noodle, draw solid, slightly transparent
            if (input == null)
            {
                Color a = GetTypeColor(output.ValueType);
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(a, 0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f) }
                );
            }
            // If normal, draw gradient fading from one input color to the other
            else
            {
                Color a = GetTypeColor(output.ValueType);
                Color b = GetTypeColor(input.ValueType);
                // If any port is hovered, tint white
                if (window.hoveredPort == output || window.hoveredPort == input)
                {
                    a = Color.Lerp(a, Color.white, 0.8f);
                    b = Color.Lerp(b, Color.white, 0.8f);
                }

                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(a, 0f), new GradientColorKey(b, 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                );
            }

            return grad;
        }

        /// <summary> Returned float is used for noodle thickness </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual float GetNoodleThickness(GUIGraphNodePort output, GUIGraphNodePort input)
        {
            return 5f;
        }

        public virtual GUIGraphConnectionPath GetNoodlePath(GUIGraphNodePort output, GUIGraphNodePort input)
        {
            return GUIGraphPreferences.GetSettings().guiGraphConnectionPath;
        }

        public virtual GUIGraphConnectionStroke GetNoodleStroke(GUIGraphNodePort output, GUIGraphNodePort input)
        {
            return GUIGraphPreferences.GetSettings().guiGraphConnectionStroke;
        }

        /// <summary> Returned color is used to color ports </summary>
        public virtual Color GetPortColor(GUIGraphNodePort port)
        {
            return GetTypeColor(port.ValueType);
        }

        /// <summary> Returns generated color for a type. This color is editable in preferences </summary>
        public virtual Color GetTypeColor(Type type)
        {
            return GUIGraphPreferences.GetTypeColor(type);
        }

        /// <summary> Override to display custom tooltips </summary>
        public virtual string GetPortTooltip(GUIGraphNodePort port)
        {
            Type portType = port.ValueType;
            string tooltip = "";
            tooltip = portType.PrettyName();
            if (port.IsOutput)
            {
                object obj = port.node.GetValue(port);
                tooltip += " = " + (obj != null ? obj.ToString() : "null");
            }

            return tooltip;
        }

        /// <summary> Deal with objects dropped into the graph through DragAndDrop </summary>
        public virtual void OnDropObjects(UnityEngine.Object[] objects)
        {
            if (GetType() != typeof(GUIGraphEditor)) Debug.Log("No OnDropObjects override defined for " + GetType());
        }

        /// <summary> Create a node and save it in the graph asset </summary>
        public virtual GUIGraphNode CreateNode(Type type, Vector2 position)
        {
            Undo.RecordObject(target, "Create Node");
            GUIGraphNode node = target.AddNode(type);
            Undo.RegisterCreatedObjectUndo(node, "Create Node");
            node.position = position;
            if (node.name == null || node.name.Trim() == "")
                node.name = GUIGraphUtilities.NodeDefaultName(type);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target))) AssetDatabase.AddObjectToAsset(node, target);
            if (GUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            GUIGraphWindow.RepaintAll();
            return node;
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual GUIGraphNode CopyNode(GUIGraphNode original)
        {
            Undo.RecordObject(target, "Duplicate Node");
            GUIGraphNode node = target.CopyNode(original);
            Undo.RegisterCreatedObjectUndo(node, "Duplicate Node");
            node.name = original.name;
            AssetDatabase.AddObjectToAsset(node, target);
            if (GUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            return node;
        }

        /// <summary> Return false for nodes that can't be removed </summary>
        public virtual bool CanRemove(GUIGraphNode node)
        {
            // Check graph attributes to see if this node is required
            Type graphType = target.GetType();
            GUIGraph.RequireNodeAttribute[] attribs = Array.ConvertAll(
                graphType.GetCustomAttributes(typeof(GUIGraph.RequireNodeAttribute), true),
                x => x as GUIGraph.RequireNodeAttribute);
            if (attribs.Any(x => x.Requires(node.GetType())))
            {
                if (target.nodes.Count(x => x.GetType() == node.GetType()) <= 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Safely remove a node and all its connections. </summary>
        public virtual void RemoveNode(GUIGraphNode node)
        {
            if (!CanRemove(node)) return;

            // Remove the node
            Undo.RecordObject(node, "Delete Node");
            Undo.RecordObject(target, "Delete Node");
            foreach (var port in node.Ports)
            foreach (var conn in port.GetConnections())
                Undo.RecordObject(conn.node, "Delete Node");
            target.RemoveNode(node);
            Undo.DestroyObjectImmediate(node);
            if (GUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }


        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeGraphEditorAttribute : Attribute,
            Internal.GUIGraphEditorBase<GUIGraphEditor, GUIGraphEditor.CustomNodeGraphEditorAttribute,
                GUIGraph>.INodeEditorAttrib
        {
            private Type inspectedType;
            public string editorPrefsKey;

            /// <summary> Tells a NodeGraphEditor which Graph type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            /// <param name="editorPrefsKey">Define unique key for unique layout settings instance</param>
            public CustomNodeGraphEditorAttribute(Type inspectedType, string editorPrefsKey = "xNode.Settings")
            {
                this.inspectedType = inspectedType;
                this.editorPrefsKey = editorPrefsKey;
            }

            public Type GetInspectedType()
            {
                return inspectedType;
            }
        }
    }
}
#endif