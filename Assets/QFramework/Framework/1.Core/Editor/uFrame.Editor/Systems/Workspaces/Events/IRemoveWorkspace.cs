namespace QF.GraphDesigner
{
    public interface IRemoveWorkspace
    {
        void RemoveWorkspace(string name);
        void RemoveWorkspace(Workspace workspace);
    }
}