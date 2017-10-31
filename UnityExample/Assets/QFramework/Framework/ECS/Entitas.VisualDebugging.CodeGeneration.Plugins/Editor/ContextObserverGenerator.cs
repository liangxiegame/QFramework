using System.Linq;
using QFramework.CodeGeneration;
using QFramework.CodeGeneration.Plugins;



namespace QFramework.VisualDebugging.CodeGeneration.Plugins
{

    public class ContextObserverGenerator : ICodeGenerator
    {

        public string Name
        {
            get { return "Context Observer"; }
        }

        public int Priority
        {
            get { return 0; }
        }

        public bool IsEnabledByDefault
        {
            get { return true; }
        }

        public bool RunInDryMode
        {
            get { return true; }
        }

        const string CONTEXTS_TEMPLATE =
            @"public partial class Contexts 
            {
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

    [QFramework.CodeGeneration.Attributes.PostConstructor]
    public void InitializeContexObservers() 
    {
        try 
        {
            ${contextObservers}
        } catch(System.Exception) 
        {
        }
    }

    public void CreateContextObserver(QFramework.IContext context) 
    {
        if (UnityEngine.Application.isPlaying) 
        {
            var observer = new QFramework.VisualDebugging.Unity.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.GameObject);
        }
    }

#endif
}
";

        const string CONTEXT_OBSERVER_TEMPLATE = @"            CreateContextObserver(${contextName});";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var contextNames = data
                .OfType<ContextData>()
                .Select(d => d.GetContextName())
                .OrderBy(contextName => contextName)
                .ToArray();

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    generateContextsClass(contextNames),
                    GetType().FullName)
            };
        }

        string generateContextsClass(string[] contextNames)
        {
            var contextObservers = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_OBSERVER_TEMPLATE
                    .Replace("${contextName}", contextName.LowercaseFirst())
                ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextObservers}", contextObservers);
        }
    }
}