using System.Collections.Generic;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public abstract class Drawer : IItemDrawer
    {

        private object _dataContext;
        private List<IDrawer> _children;

        protected Drawer()
        {

        }

        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                _dataContext = value;
                if (_dataContext != null)
                {
                    DataContextChanged();
                }
            }
        }

        protected virtual void DataContextChanged()
        {
            
        }

        public GraphItemViewModel ViewModelObject
        {
            get { return DataContext as GraphItemViewModel; }
            set { DataContext = value; }
        }

        public virtual Rect Bounds { get; set; }

        public bool Enabled
        {
            get { return this.ViewModelObject.Enabled; }
            set { }
        }

        public virtual bool IsSelected
        {
            get { return ViewModelObject.IsSelected; }
            set { ViewModelObject.IsSelected = value; }
        }

        public bool Dirty { get; set; }
        public string ShouldFocus { get; set; }

        protected Drawer(GraphItemViewModel viewModelObject)
        {
            ViewModelObject = viewModelObject;
        }

        public virtual void Draw(IPlatformDrawer platform, float scale)
        {
        }

        public virtual void Refresh(IPlatformDrawer platform)
        {
            if (ViewModelObject == null)
            {
                Refresh(platform, Vector2.zero);
            }
            else
            {
                Refresh(platform, Bounds.position);
            }

        }

        public virtual void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {

        }

        public virtual int ZOrder { get { return 0; } }

        public List<IDrawer> Children
        {
            get { return _children ?? (_children = new List<IDrawer>()); }
            set { _children = value; }
        }

        public IDrawer ParentDrawer { get; set; }


        public virtual void OnDeselecting()
        {

        }

        public virtual void OnSelecting()
        {

        }

        public virtual void OnDeselected()
        {


        }

        public virtual void OnSelected()
        {
        }

        public virtual void OnMouseExit(MouseEvent e)
        {
            InvertApplication.SignalEvent<IOnMouseExitEvent>(_ => _.OnMouseExit(this, e));
            ViewModelObject.IsMouseOver = false;
        }

        public virtual void OnMouseEnter(MouseEvent e)
        {
            ViewModelObject.IsMouseOver = true;
            InvertApplication.SignalEvent<IOnMouseEnterEvent>(_ => _.OnMouseEnter(this, e));
        }

        public virtual void OnMouseMove(MouseEvent e)
        {
        }

        public virtual void OnDrag(MouseEvent e)
        {

            InvertApplication.SignalEvent<IOnDragEvent>(_ => _.OnDrag(this, e));
        }

        public virtual void OnMouseUp(MouseEvent e)
        {
            InvertApplication.SignalEvent<IOnMouseUpEvent>(_ => _.OnMouseUp(this, e));
        }

        public virtual void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
            InvertApplication.SignalEvent<IOnMouseDoubleClickEvent>(_ => _.OnMouseDoubleClick(this, mouseEvent));
        }

        public virtual void OnRightClick(MouseEvent mouseEvent)
        {
            InvertApplication.SignalEvent<IOnRightClickEvent>(_ => _.OnRightClick(this, mouseEvent));
        }

        public virtual void OnMouseDown(MouseEvent mouseEvent)
        {
            InvertApplication.SignalEvent<IOnMouseDownEvent>(_ => _.OnMouseDown(this, mouseEvent));
        }

        public virtual void OnLayout()
        {

        }
    }
}