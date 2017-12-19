

namespace QFramework.VisualDebugging.CodeGeneration.Plugins
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

public class Feature : QFramework.VisualDebugging.Unity.DebugSystems 
{

    public Feature(string name) : base(name) 
    {
    }

    public Feature() : base(true) 
    {
        var typeName = QFramework.TypeSerializationExtension.ToCompilableString(GetType());
        var shortType = QFramework.TypeSerializationExtension.ShortTypeName(typeName);
        var readableType = QFramework.StringExtension.ToSpacedCamelCase(shortType);

        initialize(readableType);
    }
}

#else

public class Feature : QFramework.Systems 
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