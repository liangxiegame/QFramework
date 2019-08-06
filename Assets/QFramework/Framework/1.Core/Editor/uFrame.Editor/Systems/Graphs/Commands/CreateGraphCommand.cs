using System;
using QF.GraphDesigner;

public class CreateGraphCommand : Command
{
    [InspectorProperty]
    public string Name { get; set; }
    public Type GraphType { get; set; }
}