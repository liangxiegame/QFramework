using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class SubWindow : EditorWindow, ILayout
    {
        void IView.Hide()
        {
        }

        void IView.DrawGUI()
        {
        }

        ILayout IView. Parent          { get; set; }
        GUIStyle IView.Style { get; }
        Color IView.   BackgroundColor { get; set; }


        private List<IView> mPrivateChildren = new List<IView>();

        private List<IView> mChildren
        {
            get { return mPrivateChildren; }
            set { mPrivateChildren = value; }
        }

        void IView.RefreshNextFrame()
        {
        }

        void IView.AddLayoutOption(GUILayoutOption option)
        {
        }

        void IView.RemoveFromParent()
        {
        }

        void IView.Refresh()
        {
        }

        public void AddChild(IView view)
        {
            mChildren.Add(view);
            view.Parent = this;
        }

        public void RemoveChild(IView view)
        {
            mChildren.Add(view);
            view.Parent = null;
        }

        public void Clear()
        {
            mChildren.Clear();
        }

        private void OnGUI()
        {
            mChildren.ForEach(view => view.DrawGUI());
        }

        public void Dispose()
        {
        }
    }
}