using System;
using System.Collections.Generic;
using UnityEngine;

namespace EGO.Framework
{
    public abstract class Layout : View,ILayout
    {
        protected List<IView> Children = new List<IView>();

        public void AddChild(IView view)
        {
            Children.Add(view);
            view.Parent = this;
        }

        public void RemoveChild(IView view)
        {
            this.PushCommand(() =>
            {
                Children.Remove(view);
                view.Parent = null;
            });

            view.Dispose();
        }

        public void Clear()
        {
            Children.Clear();
        }

        public override void Refresh()
        {
            Children.ForEach(view=>view.Refresh());
            base.Refresh();
        }

        protected override void OnGUI()
        {
            OnGUIBegin();
            foreach (var child in Children)
            {
                child.DrawGUI();
            }
            OnGUIEnd();
        }

        protected abstract void OnGUIBegin();
        protected abstract void OnGUIEnd();
    }
}