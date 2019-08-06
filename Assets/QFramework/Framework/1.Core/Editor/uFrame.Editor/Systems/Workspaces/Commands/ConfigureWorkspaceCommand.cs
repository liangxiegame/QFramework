using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class ConfigureWorkspaceCommand : ICommand
    {
        public string Title { get; set; }

        [InspectorProperty]
        public string Name { get; set; }

        public Workspace Workspace { get; set; }

    }
}