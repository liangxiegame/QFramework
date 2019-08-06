using Invert.uFrame.Editor;

namespace QF.GraphDesigner
{
    public interface IInputHandler
    {
        void OnMouseDoubleClick(MouseEvent mouseEvent);
        void OnMouseDown(MouseEvent mouseEvent);
        void OnMouseMove(MouseEvent e);
        void OnMouseUp(MouseEvent mouseEvent);
        void OnRightClick(MouseEvent mouseEvent);
        void Draw(IPlatformDrawer platform, float scale);
    }
}