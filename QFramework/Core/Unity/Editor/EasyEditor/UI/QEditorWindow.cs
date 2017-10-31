
using System.Collections.Generic;

namespace QFramework
{
    using UnityEditor;

    public class QEditorWindow : EditorWindow ,IEditorView
    {
        List<IEditorView> mChildren = new List<IEditorView>();

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public void AddChild(IEditorView childView)
        {
            mChildren.Add(childView);
        }

        public void RemoveChild(IEditorView childView)
        {
            mChildren.Remove(childView);
        }

        public void OnGUI()
        {
            if (Visible) mChildren.ForEach(childView => childView.OnGUI());
        }
    }
}