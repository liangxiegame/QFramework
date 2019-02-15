using QFramework.GraphDesigner;

namespace QFramework.GraphDesigner
{
    public class ConfigureWorkspaceCommand : ICommand
    {
        public string Title { get; set; }

        [InspectorProperty]
        public string Name { get; set; }

        public Workspace Workspace { get; set; }

    }
}