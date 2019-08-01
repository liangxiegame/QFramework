using System;

namespace QFramework.GraphDesigner
{
    public interface ITutorialStep
    {
        string Name { get; set; }
        Action DoIt { get; set; }
        Func<string> IsDone { get; set; }
        Action<IDocumentationBuilder> StepContent { get; set; }
        string IsComplete { get; set; }
    }
}