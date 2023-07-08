/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    /// <summary> Base class to derive custom Node Graph editors from. Use this to override how graphs are drawn in the editor. </summary>
    [CustomNodeGraphEditor(typeof(IMGUIGraph))]
    public class IMGUIGraphEditor : Internal.IMGUIGraphEditorBase<IMGUIGraphEditor, IMGUIGraphEditor.CustomNodeGraphEditorAttribute, IMGUIGraph>
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
            return IMGUIGraphPreferences.GetSettings().gridTexture;
        }

        public virtual Texture2D GetSecondaryGridTexture()
        {
            return IMGUIGraphPreferences.GetSettings().crossTexture;
        }

        /// <summary> Return default settings for this graph type. This is the settings the user will load if no previous settings have been saved. </summary>
        public virtual IMGUIGraphPreferences.Settings GetDefaultPreferences()
        {
            return new IMGUIGraphPreferences.Settings();
        }

        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
        public virtual string GetNodeMenuName(Type type)
        {
            //Check if type has the CreateNodeMenuAttribute
            IMGUIGraphNode.CreateNodeMenuAttribute attrib;
            if (IMGUIGraphUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.menuName;
            else // Return generated path
                return IMGUIGraphUtilities.NodeDefaultPath(type);
        }

        /// <summary> The order by which the menu items are displayed. </summary>
        public virtual int GetNodeMenuOrder(Type type)
        {
            //Check if type has the CreateNodeMenuAttribute
            IMGUIGraphNode.CreateNodeMenuAttribute attrib;
            if (IMGUIGraphUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.order;
            else
                return 0;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu)
        {
            Vector2 pos = IMGUIGraphWindow.current.WindowToGridPosition(Event.current.mousePosition);
            var nodeTypes = IMGUIGraphReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            for (int i = 0; i < nodeTypes.Length; i++)
            {
                Type type = nodeTypes[i];

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type
                IMGUIGraphNode.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (IMGUIGraphUtilities.GetAttrib(type, out disallowAttrib))
                {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                // Add node entry to context menu
                if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                else
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        IMGUIGraphNode node = CreateNode(type, pos);
                        IMGUIGraphWindow.current.AutoConnect(node);
                    });
            }

            menu.AddSeparator("");
            if (IMGUIGraphWindow.copyBuffer != null && IMGUIGraphWindow.copyBuffer.Length > 0)
                menu.AddItem(new GUIContent("Paste"), false, () => IMGUIGraphWindow.current.PasteNodes(pos));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            // menu.AddItem(new GUIContent("Preferences"), false, () => IMGUIGraphReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }

        /// <summary> Returned gradient is used to color noodles </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual Gradient GetNoodleGradient(IMGUIGraphNodePort output, IMGUIGraphNodePort input)
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
        public virtual float GetNoodleThickness(IMGUIGraphNodePort output, IMGUIGraphNodePort input)
        {
            return 5f;
        }

        public virtual IMGUIGraphConnectionPath GetNoodlePath(IMGUIGraphNodePort output, IMGUIGraphNodePort input)
        {
            return IMGUIGraphPreferences.GetSettings().imguiGraphConnectionPath;
        }

        public virtual IMGUIGraphConnectionStroke GetNoodleStroke(IMGUIGraphNodePort output, IMGUIGraphNodePort input)
        {
            return IMGUIGraphPreferences.GetSettings().imguiGraphConnectionStroke;
        }

        /// <summary> Returned color is used to color ports </summary>
        public virtual Color GetPortColor(IMGUIGraphNodePort port)
        {
            return GetTypeColor(port.ValueType);
        }

        /// <summary> Returns generated color for a type. This color is editable in preferences </summary>
        public virtual Color GetTypeColor(Type type)
        {
            return IMGUIGraphPreferences.GetTypeColor(type);
        }

        /// <summary> Override to display custom tooltips </summary>
        public virtual string GetPortTooltip(IMGUIGraphNodePort port)
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
            if (GetType() != typeof(IMGUIGraphEditor)) Debug.Log("No OnDropObjects override defined for " + GetType());
        }

        /// <summary> Create a node and save it in the graph asset </summary>
        public virtual IMGUIGraphNode CreateNode(Type type, Vector2 position)
        {
            Undo.RecordObject(target, "Create Node");
            IMGUIGraphNode node = target.AddNode(type);
            Undo.RegisterCreatedObjectUndo(node, "Create Node");
            node.position = position;
            if (node.name == null || node.name.Trim() == "")
                node.name = IMGUIGraphUtilities.NodeDefaultName(type);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target))) AssetDatabase.AddObjectToAsset(node, target);
            if (IMGUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            IMGUIGraphWindow.RepaintAll();
            return node;
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual IMGUIGraphNode CopyNode(IMGUIGraphNode original)
        {
            Undo.RecordObject(target, "Duplicate Node");
            IMGUIGraphNode node = target.CopyNode(original);
            Undo.RegisterCreatedObjectUndo(node, "Duplicate Node");
            node.name = original.name;
            AssetDatabase.AddObjectToAsset(node, target);
            if (IMGUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            return node;
        }

        /// <summary> Return false for nodes that can't be removed </summary>
        public virtual bool CanRemove(IMGUIGraphNode node)
        {
            // Check graph attributes to see if this node is required
            Type graphType = target.GetType();
            IMGUIGraph.RequireNodeAttribute[] attribs = Array.ConvertAll(
                graphType.GetCustomAttributes(typeof(IMGUIGraph.RequireNodeAttribute), true),
                x => x as IMGUIGraph.RequireNodeAttribute);
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
        public virtual void RemoveNode(IMGUIGraphNode node)
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
            if (IMGUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }


        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeGraphEditorAttribute : Attribute,
            Internal.IMGUIGraphEditorBase<IMGUIGraphEditor, IMGUIGraphEditor.CustomNodeGraphEditorAttribute,
                IMGUIGraph>.INodeEditorAttrib
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