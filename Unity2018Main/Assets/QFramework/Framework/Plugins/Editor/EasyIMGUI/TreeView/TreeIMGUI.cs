using UnityEngine;
using UnityEditor;

/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

namespace QFramework.TreeView
{
    public class TreeIMGUI<T> where T : ITreeIMGUIData
    {
        private readonly TreeNode<T> _root;

        private Rect _controlRect;
        private float _drawY;
        private float _height;
        private TreeNode<T> _selected;
        private int _controlID;

        public TreeIMGUI(TreeNode<T> root)
        {
            _root = root;
        }

        public void DrawTreeLayout()
        {
            _height = 0;
            _drawY = 0;
            _root.Traverse(OnGetLayoutHeight);

            _controlRect = EditorGUILayout.GetControlRect(false, _height);
            _controlID = GUIUtility.GetControlID(FocusType.Passive, _controlRect);
            _root.Traverse(OnDrawRow);
        }

        protected virtual float GetRowHeight(TreeNode<T> node)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual bool OnGetLayoutHeight(TreeNode<T> node)
        {
            if (node.Data == null) return true;

            _height += GetRowHeight(node);
            return node.Data.isExpanded;
        }

        protected virtual bool OnDrawRow(TreeNode<T> node)
        {
            if (node.Data == null) return true;

            float rowIndent = 14 * node.Level;
            float rowHeight = GetRowHeight(node);

            Rect rowRect = new Rect(0, _controlRect.y + _drawY, _controlRect.width, rowHeight);
            Rect indentRect = new Rect(rowIndent, _controlRect.y + _drawY, _controlRect.width - rowIndent, rowHeight);

            // render
            if (_selected == node)
            {
                EditorGUI.DrawRect(rowRect, Color.gray);
            }

            OnDrawTreeNode(indentRect, node, _selected == node, false);

            // test for events
            EventType eventType = Event.current.GetTypeForControl(_controlID);
            if (eventType == EventType.MouseUp && rowRect.Contains(Event.current.mousePosition))
            {
                _selected = node;

                GUI.changed = true;
                Event.current.Use();
            }

            _drawY += rowHeight;

            return node.Data.isExpanded;
        }

        protected virtual void OnDrawTreeNode(Rect rect, TreeNode<T> node, bool selected, bool focus)
        {
            GUIContent labelContent = new GUIContent(node.Data.ToString());

            if (!node.IsLeaf)
            {
                node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x - 12, rect.y, 12, rect.height),
                    node.Data.isExpanded, GUIContent.none);
            }

            EditorGUI.LabelField(rect, labelContent, selected ? EditorStyles.whiteLabel : EditorStyles.label);
        }
    }

    public interface ITreeIMGUIData
    {
        bool isExpanded { get; set; }
    }
}
