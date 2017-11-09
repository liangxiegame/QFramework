using System.IO;
using System.Linq;


namespace QFramework {

    public class ContextMatcherGenerator : ICodeGenerator {

        public string Name { get { return "Context (Matcher API)"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        const string CONTEXT_TEMPLATE =
@"public sealed partial class ${ContextName}Matcher {

    public static QFramework.IAllOfMatcher<${ContextName}Entity> AllOf(params int[] indices) {
        return QFramework.Matcher<${ContextName}Entity>.AllOf(indices);
    }

    public static QFramework.IAllOfMatcher<${ContextName}Entity> AllOf(params QFramework.IMatcher<${ContextName}Entity>[] matchers) {
          return QFramework.Matcher<${ContextName}Entity>.AllOf(matchers);
    }

    public static QFramework.IAnyOfMatcher<${ContextName}Entity> AnyOf(params int[] indices) {
          return QFramework.Matcher<${ContextName}Entity>.AnyOf(indices);
    }

    public static QFramework.IAnyOfMatcher<${ContextName}Entity> AnyOf(params QFramework.IMatcher<${ContextName}Entity>[] matchers) {
          return QFramework.Matcher<${ContextName}Entity>.AnyOf(matchers);
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateContextClass(d))
                .ToArray();
        }

        CodeGenFile generateContextClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Matcher.cs",
                CONTEXT_TEMPLATE
                    .Replace("${ContextName}", contextName),
                GetType().FullName
            );
        }
    }
}
