using System.Collections.Generic;
using UnityEngine;

namespace QF
{
    public class EditorView : IEditorView
    {
        #region GUILayoutOptions

        protected float mWidth = 0.0f;
        public float Width
        {
            get { return mWidth; }
            set
            {
                mWidth = value;
                mLayoutOptions[0] = GUILayout.Width(mWidth);
            }
        }
        
        protected float mHeight = 0.0f;
        
        public float Height
        {
            get { return mHeight; }
            set
            {
                mHeight = value;
                mLayoutOptions[1] = GUILayout.Height(mHeight);
            }
        }

        protected GUILayoutOption[] mLayoutOptions = new GUILayoutOption[2];
        #endregion
        
        private readonly List<IEditorView> mChildren = new List<IEditorView>();

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public virtual void AddChild(IEditorView editorView)
        {
            mChildren.Add(editorView);
        }

        public virtual void RemoveChild(IEditorView editorView)
        {
            mChildren.Remove(editorView);
        }

        
        public virtual void OnGUI()
        {
            if (!Visible) return;
            mChildren.ForEach(childView => childView.OnGUI());
        }
    }
}