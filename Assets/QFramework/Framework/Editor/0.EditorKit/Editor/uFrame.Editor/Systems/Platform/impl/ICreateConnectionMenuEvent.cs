namespace QFramework.GraphDesigner
{
    public interface ICreateConnectionMenuEvent
    {
        void CreateConnectionMenu(ConnectionHandler viewModel, DiagramViewModel diagramViewModel, MouseEvent mouseEvent);
    }
}