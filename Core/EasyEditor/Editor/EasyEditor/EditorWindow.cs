
using System.Collections.Generic;

namespace QFramework
{
    using UnityEditor;

    public class QEditorWindow : EditorWindow ,IEditorView
    {
        public static T Create<T>(bool utility,string title = null) where T : QEditorWindow
        {
            return title.IsNullOrEmpty() ? GetWindow<T>(utility) : GetWindow<T>(utility,title);
        }
        
        private readonly List<IEditorView> mChildren = new List<IEditorView>();

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

        public virtual void OnGUI()
        {
            if (Visible) mChildren.ForEach(childView => childView.OnGUI());
        }
    }
}