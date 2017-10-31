using System.Collections.Generic;
using System.Linq;



namespace QFramework.CodeGeneration.Plugins {

    public class ContextDataProvider : ICodeGeneratorDataProvider, IConfigurable {

        public string Name { get { return "Context"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        public Dictionary<string,string> DefaultProperties { get { return _contextNamesConfig.DefaultProperties; } }

        readonly ContextNamesConfig _contextNamesConfig = new ContextNamesConfig();

        public void Configure(Properties properties) {
            _contextNamesConfig.Configure(properties);
        }

        public CodeGeneratorData[] GetData() {
            return _contextNamesConfig.contextNames
                .Select(contextName => {
                    var data = new ContextData();
                    data.SetContextName(contextName);
                    return data;
                }).ToArray();
        }
    }

    public static class ContextDataExtension {

        public const string CONTEXT_NAME = "context_name";

        public static string GetContextName(this ContextData data) {
            return (string)data[CONTEXT_NAME];
        }

        public static void SetContextName(this ContextData data, string contextName) {
            data[CONTEXT_NAME] = contextName;
        }
    }
}