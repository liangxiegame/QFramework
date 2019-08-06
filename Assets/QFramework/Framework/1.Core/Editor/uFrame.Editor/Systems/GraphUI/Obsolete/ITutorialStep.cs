using System;

namespace QF.GraphDesigner
{
    public interface ITutorialStep
    {
        string Name { get; set; }
        System.Action DoIt { get; set; }
        Func<string> IsDone { get; set; }
        Action<IDocumentationBuilder> StepContent { get; set; }
        string IsComplete { get; set; }
    }
}