using System.IO;
using System.Linq;


namespace QFramework.CodeGeneration.Plugins
{

    public class ContextGenerator : ICodeGenerator
    {

        public string Name
        {
            get { return "Context"; }
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

        const string CONTEXT_TEMPLATE =
            @"public sealed partial class ${ContextName}Context : QFramework.Context<${ContextName}Entity> 
  {

    public ${ContextName}Context()
        : base(
            ${Lookup}.TotalComponents,
            0,
            new QFramework.ContextInfo(
                ""${ContextName}"",
                ${Lookup}.componentNames,
                ${Lookup}.componentTypes
            ),
            (entity) =>

#if (ENTITAS_FAST_AND_UNSAFE)
                new QFramework.UnsafeARC()
#else
                new QFramework.SafeARC(entity)
#endif

        ) {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            return data
                .OfType<ContextData>()
                .Select(d => generateContextClass(d))
                .ToArray();
        }

        CodeGenFile generateContextClass(ContextData data)
        {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Context.cs",
                CONTEXT_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${Lookup}", contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP),
                GetType().FullName
            );
        }
    }
}