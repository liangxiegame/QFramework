using System.Collections.Generic;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    public class UnityWindowDrawer : EditorWindow, IDrawer {
        private IPlatformDrawer _platformDrawer;
        private List<IDrawer> _children;
        private GraphItemViewModel _viewModelObject;

        public IPlatformDrawer PlatformDrawer
        {
            get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platformDrawer = value; }
        }

        void OnGUI()
        {
            if (ViewModelObject == null) return;
            Draw(PlatformDrawer,1.0f);
        }

        void Update()
        {
            Repaint();
        }

        public GraphItemViewModel ViewModelObject
        {
            get { return _viewModelObject; }
            set
            {
                _viewModelObject = value; 
                RefreshContent(Children);
            }
        }

        public Rect Bounds
        {
            get { return this.position; }
            set { }
        }

        public bool Enabled
        {
            get { return true; }
            set { }
        }

        public bool IsSelected { get; set; }

        public bool Dirty { get; set; }

        public string ShouldFocus { get; set; }


        public void RefreshContent(List<IDrawer> children)
        {
            children.Clear();
            children.Add(InvertApplication.Container.CreateDrawer(ViewModelObject));
        }

        public void Draw(IPlatformDrawer platform, float scale)
        {
            Refresh(PlatformDrawer,new Vector2(0,0));
            DrawChildren();
        }

        public void Refresh(IPlatformDrawer platform)
        {
        }

        public void OnLayout()
        {
        }

        public void DrawChildren()
        {
            foreach (var item in Children)
            {
                if (item.Dirty)
                {
                    Refresh(PlatformDrawer, item.Bounds.position, false);
                    item.Dirty = false;
                }
                item.Draw(PlatformDrawer, 1.0f);
            }
        }

        public void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            foreach (var child in Children)
            {
                child.Refresh(platform, new Vector2(10, 0), hardRefresh);
            }

            foreach (var child in Children)
            {
                child.OnLayout();
            }
        }

        public int ZOrder { get; private set; }

        public List<IDrawer> Children
        {
            get { return _children ?? (_children = new List<IDrawer>()); }
            set { _children = value; }
        }

        public IDrawer ParentDrawer { get; set; }

        public void OnDeselecting()
        {
        }

        public void OnSelecting()
        {
        }

        public void OnDeselected()
        {
        }

        public void OnSelected()
        {
        }

        public void OnMouseExit(MouseEvent e)
        {
        }

        public void OnMouseEnter(MouseEvent e)
        {
        }

        public void OnMouseMove(MouseEvent e)
        {
        }

        public void OnDrag(MouseEvent e)
        {
        }

        public void OnMouseUp(MouseEvent e)
        {
        }

        public void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
        }

        public void OnRightClick(MouseEvent mouseEvent)
        {
        }

        public void OnMouseDown(MouseEvent mouseEvent)
        {
        }
    
    }
}