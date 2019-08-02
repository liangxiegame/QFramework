
using System.Collections.Generic;

namespace QFramework
{
    using UnityEditor;

    public class QEditorWindow : EditorWindow ,IOnGUIView
    {
        public static T Create<T>(bool utility,string title = null) where T : QEditorWindow
        {
            return title.IsNullOrEmpty() ? GetWindow<T>(utility) : GetWindow<T>(utility,title);
        }
        
        private readonly List<IOnGUIView> mChildren = new List<IOnGUIView>();

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }
        
        public void AddChild(IOnGUIView childView)
        {
            mChildren.Add(childView);
        }

        public void RemoveChild(IOnGUIView childView)
        {
            mChildren.Remove(childView);
        }

        public void RemoveAllChidren()
        {
            mChildren.Clear();
        }

        public virtual void OnGUI()
        {
            if (Visible) mChildren.ForEach(childView => childView.OnGUI());
        }

        public IOnGUIView End()
        {
            throw new System.NotImplementedException();
        }
    }
}