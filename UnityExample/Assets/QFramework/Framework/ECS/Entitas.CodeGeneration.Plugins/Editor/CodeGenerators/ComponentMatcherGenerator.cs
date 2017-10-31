using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace QFramework.CodeGeneration.Plugins {

    public class ComponentMatcherGenerator : ICodeGenerator, IConfigurable {

        public string Name { get { return "Component (Matcher API)"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        public Dictionary<string,string> DefaultProperties { get { return _ignoreNamespacesConfig.DefaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string STANDARD_COMPONENT_TEMPLATE =
@"public sealed partial class ${ContextName}Matcher {

    static QFramework.IMatcher<${ContextName}Entity> _matcher${ComponentName};

    public static QFramework.IMatcher<${ContextName}Entity> ${ComponentName} {
        get {
            if (_matcher${ComponentName} == null) {
                var matcher = (QFramework.Matcher<${ContextName}Entity>)QFramework.Matcher<${ContextName}Entity>.AllOf(${Index});
                matcher.componentNames = ${ComponentNames};
                _matcher${ComponentName} = matcher;
            }

            return _matcher${ComponentName};
        }
    }
}
";

        public void Configure(Properties properties) {
            _ignoreNamespacesConfig.Configure(properties);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .SelectMany(d => generateMatcher(d))
                .ToArray();
        }

        CodeGenFile[] generateMatcher(ComponentData data) {
            return data.GetContextNames()
                       .Select(context => generateMatcher(context, data))
                       .ToArray();
        }

        CodeGenFile generateMatcher(string contextName, ComponentData data) {
            var componentName = data.GetFullTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + componentName;
            var componentNames = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + ".componentNames";

            var fileContent = STANDARD_COMPONENT_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentName}", componentName)
                .Replace("${Index}", index)
                .Replace("${ComponentNames}", componentNames);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + componentName.AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
