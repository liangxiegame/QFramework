
using System;
using System.Collections.Generic;

namespace QF
{
    using UnityEditor;

    public class QEditorWindow : EditorWindow ,IOnGUIView
    {
        public static T Create<T>(bool utility,string title = null) where T : QEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility,title);
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

        public List<IOnGUIView> Children
        {
            get { return mChildren; }
        }

        public void RemoveAllChidren()
        {
            mChildren.Clear();
        }

        public virtual void OnOpen()
        {
            
        }
        public virtual void OnClose()
        {
            
        }

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
        }


        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        public virtual void OnUpdate()
        {
            
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