using System;

namespace QFramework.GraphDesigner
{
    public class CreateWorkspaceCommand : Command
    {
        [InspectorProperty]
        public string Name { get; set; }
        public Workspace Result { get; set; }
        public Type WorkspaceType { get; set; }
    }
}