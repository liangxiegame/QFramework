using System.IO;
using System.Linq;


namespace QFramework {

    public class EntityGenerator : ICodeGenerator {

        public string Name { get { return "Entity"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        const string ENTITY_TEMPLATE =
@"public sealed partial class ${ContextName}Entity : QFramework.Entity {
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateEntityClass(d))
                .ToArray();
        }

        CodeGenFile generateEntityClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Entity.cs",
                ENTITY_TEMPLATE.Replace("${ContextName}", contextName),
                GetType().FullName
            );
        }
    }
}
