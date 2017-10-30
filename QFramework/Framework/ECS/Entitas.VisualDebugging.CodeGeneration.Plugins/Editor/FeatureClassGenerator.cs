using QFramework;

namespace Entitas.VisualDebugging.CodeGeneration.Plugins
{

    public class FeatureClassGenerator : ICodeGenerator
    {

        public string Name
        {
            get { return "Feature Class"; }
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

        const string FEATURE_TEMPLATE =
            @"#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

public class Feature : Entitas.VisualDebugging.Unity.DebugSystems 
{

    public Feature(string name) : base(name) 
    {
    }

    public Feature() : base(true) 
    {
        var typeName = Entitas.Utils.TypeSerializationExtension.ToCompilableString(GetType());
        var shortType = Entitas.Utils.TypeSerializationExtension.ShortTypeName(typeName);
        var readableType = QFramework.StringExtension.ToSpacedCamelCase(shortType);

        initialize(readableType);
    }
}

#else

public class Feature : Entitas.Systems 
{
    public Feature(string name) 
    {
    }

    public Feature() 
    {
    }
}

#endif
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            return new[]
            {
                new CodeGenFile(
                    "Feature.cs",
                    FEATURE_TEMPLATE,
                    GetType().FullName)
            };
        }
    }
}