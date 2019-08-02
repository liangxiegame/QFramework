

namespace QF.GraphDesigner
{
    public class Drawer<TViewModel> : Drawer where TViewModel : GraphItemViewModel 
    {
        private TViewModel _casted;

        public Drawer(GraphItemViewModel viewModelObject) : base(viewModelObject)
        {

        }

        public Drawer(TViewModel viewModelObject) : base(viewModelObject)
        {

        }
        
        public TViewModel ViewModel
        {
            get { return _casted ?? (_casted = (TViewModel)ViewModelObject); }
        }


        public override void OnDeselecting()
        {
            base.OnDeselecting();
        }

        public override void OnSelecting()
        {
            base.OnSelecting();
        }

        public override void OnDeselected()
        {
            base.OnDeselected();
        }

        public override void OnSelected()
        {
            base.OnSelected();
        }

        public override void OnMouseExit(MouseEvent e)
        {
            base.OnMouseExit(e);
        }

        public override void OnMouseEnter(MouseEvent e)
        {
            base.OnMouseEnter(e);
        }

        public override void OnMouseMove(MouseEvent e)
        {
            base.OnMouseMove(e);
        }

        public override void OnDrag(MouseEvent e)
        {
            base.OnDrag(e);
        }

        public override void OnMouseUp(MouseEvent e)
        {
            base.OnMouseUp(e);
        }

        public override void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
            base.OnMouseDoubleClick(mouseEvent);
        }
    }
}