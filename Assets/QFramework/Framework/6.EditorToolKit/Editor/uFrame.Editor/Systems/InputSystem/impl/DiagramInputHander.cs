using System.Linq;

namespace QF.GraphDesigner
{
    public class DiagramInputHander : IInputHandler
    {
        public GraphItemViewModel ViewModelAtMouse { get; set; }
        public DiagramViewModel DiagramViewModel { get; set; }

        public ConnectorViewModel ConnectorAtMouse { get; set; }



        public DiagramInputHander(DiagramViewModel diagramViewModel)
        {
            DiagramViewModel = diagramViewModel;
        }

        public virtual void OnMouseDoubleClick(MouseEvent e)
        {

        }

        public virtual void OnMouseDown(MouseEvent e)
        {

        }

        public virtual void OnMouseMove(MouseEvent e)
        {
            ConnectorAtMouse = null;
            ViewModelAtMouse = null;
            foreach (var source in DiagramViewModel.GraphItems.Reverse())
            {
                if (source.Bounds.Contains(e.MousePosition))
                {
                    if (source is ConnectorViewModel) ConnectorAtMouse = (ConnectorViewModel) source;
                    else ViewModelAtMouse = ConnectorAtMouse;
                }
                if(ConnectorAtMouse != null) break;
            }

        }

        public virtual void OnMouseUp(MouseEvent e)
        {

        }

        public void OnRightClick(MouseEvent mouseEvent)
        {

        }

        public virtual void Draw(IPlatformDrawer platform, float scale)
        {
            
        }
    }
}