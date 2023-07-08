/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace QFramework.Pro
{
    [InitializeOnLoad]
    public partial class IMGUIGraphWindow : EditorWindow, IUnRegisterList
    {
        public static IMGUIGraphWindow current;

        /// <summary> Stores node positions for all nodePorts. </summary>
        public Dictionary<IMGUIGraphNodePort, Rect> portConnectionPoints
        {
            get { return _portConnectionPoints; }
        }

        private Dictionary<IMGUIGraphNodePort, Rect> _portConnectionPoints = new Dictionary<IMGUIGraphNodePort, Rect>();
        [SerializeField] private NodePortReference[] _references = new NodePortReference[0];
        [SerializeField] private Rect[] _rects = new Rect[0];

        private Func<bool> isDocked
        {
            get
            {
                if (_isDocked == null) _isDocked = this.GetIsDockedDelegate();
                return _isDocked;
            }
        }

        private Func<bool> _isDocked;

        [System.Serializable]
        private class NodePortReference
        {
            [SerializeField] private IMGUIGraphNode _node;
            [SerializeField] private string _name;

            public NodePortReference(IMGUIGraphNodePort nodePort)
            {
                _node = nodePort.node;
                _name = nodePort.fieldName;
            }

            public IMGUIGraphNodePort GetNodePort()
            {
                if (_node == null)
                {
                    return null;
                }

                return _node.GetPort(_name);
            }
        }

        private void OnDisable()
        {
            // Cache portConnectionPoints before serialization starts
            int count = portConnectionPoints.Count;
            _references = new NodePortReference[count];
            _rects = new Rect[count];
            int index = 0;
            foreach (var portConnectionPoint in portConnectionPoints)
            {
                _references[index] = new NodePortReference(portConnectionPoint.Key);
                _rects[index] = portConnectionPoint.Value;
                index++;
            }

            this.UnRegisterAll();
        }

        private void OnEnable()
        {
            // Reload portConnectionPoints if there are any
            int length = _references.Length;
            if (length == _rects.Length)
            {
                for (int i = 0; i < length; i++)
                {
                    IMGUIGraphNodePort nodePort = _references[i].GetNodePort();
                    if (nodePort != null)
                        _portConnectionPoints.Add(nodePort, _rects[i]);
                }
            }

            LocaleKitEditor.IsCN.RegisterWithInitValue(_ => { graphEditor?.OnLanguageChanged(); })
                .AddToUnregisterList(this);
        }

        public Dictionary<IMGUIGraphNode, Vector2> nodeSizes
        {
            get { return _nodeSizes; }
        }

        private Dictionary<IMGUIGraphNode, Vector2> _nodeSizes = new Dictionary<IMGUIGraphNode, Vector2>();
        public IMGUIGraph graph;

        public Vector2 panOffset
        {
            get { return _panOffset; }
            set
            {
                _panOffset = value;
                Repaint();
            }
        }

        private Vector2 _panOffset;

        public float zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = Mathf.Clamp(value, IMGUIGraphPreferences.GetSettings().minZoom,
                    IMGUIGraphPreferences.GetSettings().maxZoom);
                Repaint();
            }
        }

        private float _zoom = 1;

        void OnFocus()
        {
            current = this;
            ValidateGraphEditor();
            if (graphEditor != null)
            {
                graphEditor.OnWindowFocus();
                if (IMGUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            }

            dragThreshold = Math.Max(1f, Screen.width / 1000f);
        }

        void OnLostFocus()
        {
            if (graphEditor != null) graphEditor.OnWindowFocusLost();
        }

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        /// <summary> Handle Selection Change events</summary>
        private static void OnSelectionChanged()
        {
            IMGUIGraph nodeGraph = Selection.activeObject as IMGUIGraph;
            if (nodeGraph && !AssetDatabase.Contains(nodeGraph))
            {
                Open(nodeGraph);
            }
        }

        /// <summary> Make sure the graph editor is assigned and to the right object </summary>
        private void ValidateGraphEditor()
        {
            IMGUIGraphEditor graphEditor = IMGUIGraphEditor.GetEditor(graph, this);
            if (this.graphEditor != graphEditor && graphEditor != null)
            {
                this.graphEditor = graphEditor;
                graphEditor.OnOpen();
            }
        }

        /// <summary> Create editor window </summary>
        public static IMGUIGraphWindow Init()
        {
            IMGUIGraphWindow w = CreateInstance<IMGUIGraphWindow>();
            w.titleContent = new GUIContent("GraphKit.IMGUI");
            w.wantsMouseMove = true;
            w.Show();
            return w;
        }

        public void Save()
        {
            if (AssetDatabase.Contains(graph))
            {
                EditorUtility.SetDirty(graph);
                if (IMGUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            }
            else SaveAs();
        }

        public void SaveAs()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save NodeGraph", "NewNodeGraph", "asset", "");
            if (string.IsNullOrEmpty(path)) return;
            else
            {
                IMGUIGraph existingGraph = AssetDatabase.LoadAssetAtPath<IMGUIGraph>(path);
                if (existingGraph != null) AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(graph, path);
                EditorUtility.SetDirty(graph);
                if (IMGUIGraphPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            }
        }

        private void DraggableWindow(int windowID)
        {
            GUI.DragWindow();
        }

        public Vector2 WindowToGridPosition(Vector2 windowPosition)
        {
            return (windowPosition - (position.size * 0.5f) - (panOffset / zoom)) * zoom;
        }

        public Vector2 GridToWindowPosition(Vector2 gridPosition)
        {
            return (position.size * 0.5f) + (panOffset / zoom) + (gridPosition / zoom);
        }

        public Rect GridToWindowRectNoClipped(Rect gridRect)
        {
            gridRect.position = GridToWindowPositionNoClipped(gridRect.position);
            return gridRect;
        }

        public Rect GridToWindowRect(Rect gridRect)
        {
            gridRect.position = GridToWindowPosition(gridRect.position);
            gridRect.size /= zoom;
            return gridRect;
        }

        public Vector2 GridToWindowPositionNoClipped(Vector2 gridPosition)
        {
            Vector2 center = position.size * 0.5f;
            // UI Sharpness complete fix - Round final offset not panOffset
            float xOffset = Mathf.Round(center.x * zoom + (panOffset.x + gridPosition.x));
            float yOffset = Mathf.Round(center.y * zoom + (panOffset.y + gridPosition.y));
            return new Vector2(xOffset, yOffset);
        }

        public void SelectNode(IMGUIGraphNode node, bool add)
        {
            if (add)
            {
                List<Object> selection = new List<Object>(Selection.objects);
                selection.Add(node);
                Selection.objects = selection.ToArray();
            }
            else Selection.objects = new Object[] { node };
        }

        public void DeselectNode(IMGUIGraphNode node)
        {
            List<Object> selection = new List<Object>(Selection.objects);
            selection.Remove(node);
            Selection.objects = selection.ToArray();
        }

        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            IMGUIGraph nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as IMGUIGraph;
            if (nodeGraph != null)
            {
                Open(nodeGraph);
                return true;
            }

            return false;
        }


        public static IMGUIGraphWindow OpenWithGraph(IMGUIGraph graph)
        {
            if (!graph) return null;
            var window = Open(graph);
            var sceneView = GetWindow<SceneView>();
            sceneView.AddTab(window);
            return window;
        }


        /// <summary>Open the provided graph in the NodeEditor</summary>
        private static IMGUIGraphWindow Open(IMGUIGraph graph)
        {
            if (!graph) return null;
            IMGUIGraphWindow w = GetWindow(typeof(IMGUIGraphWindow), false, graph.Name, true) as IMGUIGraphWindow;
            w.wantsMouseMove = true;
            w.graph = graph;
            return w;
        }

        /// <summary> Repaint all open NodeEditorWindows. </summary>
        public static void RepaintAll()
        {
            IMGUIGraphWindow[] windows = Resources.FindObjectsOfTypeAll<IMGUIGraphWindow>();
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].Repaint();
            }
        }

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif