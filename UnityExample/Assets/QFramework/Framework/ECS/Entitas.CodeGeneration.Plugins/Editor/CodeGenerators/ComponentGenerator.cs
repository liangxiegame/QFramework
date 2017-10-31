using System.IO;
using System.Linq;



namespace QFramework.CodeGeneration.Plugins {

    public class ComponentGenerator : ICodeGenerator {

        public string Name { get { return "Component"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        const string COMPONENT_TEMPLATE =
@"${Contexts}${Unique}
public sealed partial class ${FullComponentName} : QFramework.IComponent {
    public ${Type} value;
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateComponent())
                .Select(d => generateComponentClass(d))
                .ToArray();
        }

        CodeGenFile generateComponentClass(ComponentData data) {
            var fullComponentName = data.GetFullTypeName().RemoveDots();
            var contexts = string.Join(", ", data.GetContextNames());
            var unique = data.IsUnique() ? "[QFramework.CodeGeneration.Attributes.UniqueAttribute]" : string.Empty;
            if (!string.IsNullOrEmpty(contexts)) {
                contexts = "[" + contexts + "]";
            }

            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar + fullComponentName + ".cs",
                COMPONENT_TEMPLATE
                    .Replace("${FullComponentName}", fullComponentName)
                    .Replace("${Type}", data.GetObjectType())
                    .Replace("${Contexts}", contexts)
                    .Replace("${Unique}", unique),
                GetType().FullName
            );
        }
    }
}
